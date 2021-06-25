using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Utilities;

namespace Playlister.Repositories
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<AccessTokenRepository> _logger;

        private static readonly ConcurrentDictionary<string, UserAccessToken> CachedTokens = new();

        public AccessTokenRepository(IOptions<DatabaseOptions> options, ILogger<AccessTokenRepository> logger)
        {
            _connectionString = options.Value.ConnectionString;
            _logger = logger;

            Task.Run(PopulateCache).Wait();
        }

        public async Task<UserAccessToken> AddToken(SpotifyAccessToken spotifyToken)
        {
            UserAccessToken userToken = Set(spotifyToken);

            try
            {
                const string sql =
                    "INSERT INTO AccessToken(access_token, refresh_token, expiration) VALUES(@AccessToken, @RefreshToken, @Expiration)";

                await using var conn = new SqliteConnection(_connectionString);
                await conn.ExecuteAsync(sql, userToken);
            }
            catch (Exception e)
            {
                // NOTE: if this happens, the token cache will have a token that the database doesn't, so they're out of sync
                _logger.LogError($"Token Insert failed:{Environment.NewLine}{e}");
                throw;
            }

            return userToken;
        }

        public UserAccessToken? Get(string accessToken)
        {
            _ = CachedTokens.TryGetValue(accessToken, out UserAccessToken? userAccessToken);
            _logger.LogDebug($"Result of getting {accessToken} from cache: {JsonUtility.PrettyPrint(userAccessToken)}");
            return userAccessToken;
        }

        public void RemoveAccessToken(string accessToken)
        {
            _ = CachedTokens.Remove(accessToken, out _);

            _logger.LogDebug($"Removed token {accessToken} from cache.");
        }

        #region cache tokens

        private UserAccessToken Set(SpotifyAccessToken token)
        {
            UserAccessToken userToken = CachedTokens.AddOrUpdate(token.AccessToken, token.ToUserAccessToken(), (_, b) =>
            {
                if (b == null)
                {
                    throw new ArgumentNullException(nameof(b),
                        "This should never be null. Check that Dapper settings are configured to work with underscores.");
                }

                b = token.ToUserAccessToken();
                return b;
            });

            _logger.LogDebug($"Added token to cache: {JsonUtility.PrettyPrint(userToken)}");

            return userToken;
        }

        private void ClearExpiredTokensFromCache()
        {
            foreach (var (key, userToken) in CachedTokens)
            {
                // only remove expired tokens after 6 hours
                if (userToken.Expiration >= DateTime.UtcNow.AddHours(-6)) continue;

                _ = CachedTokens.Remove(key, out _);
                _logger.LogDebug($"Removed expired token {userToken.AccessToken} from cache.");
            }
        }

        private async Task PopulateCache()
        {
            try
            {
                await using var conn = new SqliteConnection(_connectionString);
                IEnumerable<UserAccessToken>? tokens =
                    await conn.QueryAsync<UserAccessToken>("SELECT * FROM AccessToken");

                _logger.LogDebug("populating access token cache...");
                foreach (UserAccessToken token in tokens)
                {
                    SetAccessToken(token);
                    _logger.LogDebug($"Added token to cache: {JsonUtility.PrettyPrint(token)}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }

            _logger.LogDebug($"Cache populated: {JsonUtility.PrettyPrint(CachedTokens)}");
        }

        private static void SetAccessToken(UserAccessToken token)
        {
            _ = CachedTokens.AddOrUpdate(token.AccessToken, token, (_, b) =>
            {
                if (b == null) throw new ArgumentNullException(nameof(b));
                b = token;
                return b;
            });
        }

        #endregion
    }
}
