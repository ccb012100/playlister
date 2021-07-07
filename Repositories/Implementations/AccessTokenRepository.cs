using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Utilities;

namespace Playlister.Repositories.Implementations
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<AccessTokenRepository> _logger;

        private static readonly CacheObject<UserAccessToken> TokenCache;
        static AccessTokenRepository() => TokenCache = new CacheObject<UserAccessToken>();

        public AccessTokenRepository(IConnectionFactory connectionFactory, ILogger<AccessTokenRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

            TokenCache.Initialize(PopulateCache);
        }

        public async Task<UserAccessToken> AddTokenAsync(SpotifyAccessToken spotifyToken)
        {
            if (!TokenCache.Initialized) await PopulateCache();

            UserAccessToken userToken = Cache(spotifyToken);

            try
            {
                await using SqliteConnection conn = _connectionFactory.Connection;
                await conn.ExecuteAsync(
                    "INSERT INTO AccessToken(access_token, refresh_token, expiration) VALUES(@AccessToken, @RefreshToken, @Expiration)",
                    userToken);
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
            _ = TokenCache.Items.TryGetValue(accessToken, out UserAccessToken? userAccessToken);
            _logger.LogDebug($"Result of getting {accessToken} from cache: {JsonUtility.PrettyPrint(userAccessToken)}");
            return userAccessToken;
        }

        public void RemoveAccessToken(string accessToken)
        {
            _ = TokenCache.Items.Remove(accessToken, out _);

            _logger.LogDebug($"Removed token {accessToken} from cache.");
        }

        #region cache tokens

        private UserAccessToken Cache(SpotifyAccessToken token)
        {
            UserAccessToken userToken = TokenCache.Items.AddOrUpdate(token.AccessToken, token.ToUserAccessToken(),
                (_, b) => b == null ? throw new ArgumentNullException(nameof(b)) : token.ToUserAccessToken());

            _logger.LogDebug($"Added token to cache: {JsonUtility.PrettyPrint(userToken)}");

            return userToken;
        }

        private void ClearExpiredTokensFromCache()
        {
            foreach ((string key, UserAccessToken userToken) in TokenCache.Items)
            {
                // only remove expired tokens after 6 hours
                if (userToken.Expiration >= DateTime.UtcNow.AddHours(-6)) continue;

                _ = TokenCache.Items.Remove(key, out _);
                _logger.LogDebug($"Removed expired token {userToken.AccessToken} from cache.");
            }
        }

        private async Task PopulateCache()
        {
            try
            {
                await using SqliteConnection conn = _connectionFactory.Connection;
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

            _logger.LogDebug($"Cache populated: {JsonUtility.PrettyPrint(TokenCache.Items)}");
        }

        private static void SetAccessToken(UserAccessToken token)
        {
            _ = TokenCache.Items.AddOrUpdate(token.AccessToken, token, (_, b) =>
            {
                if (b == null) throw new ArgumentNullException(nameof(b));
                b = token;
                return b;
            });
        }

        #endregion
    }
}
