using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Utilities;

namespace Playlister.Services
{
    public class CacheService : ICacheService
    {
        private static readonly ConcurrentDictionary<string, UserAccessToken> AccessTokens = new();
        private readonly ILogger<CacheService> _logger;

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;
        }

        public UserAccessToken Set(SpotifyAccessToken token)
        {
            UserAccessToken userToken = AccessTokens.AddOrUpdate(token.AccessToken, token.ToUserAccessToken(), (_, b) =>
            {
                if (b == null) throw new ArgumentNullException(nameof(b));
                b = token.ToUserAccessToken();
                return b;
            });

            _logger.LogDebug($"Added token to cache: {JsonUtility.PrettyPrint(userToken)}");

            return userToken;
        }


        public UserAccessToken? Get(string accessToken)
        {
            _ = AccessTokens.TryGetValue(accessToken, out UserAccessToken? userAccessToken);
            _logger.LogDebug($"Result of getting {accessToken} from cache: {JsonUtility.PrettyPrint(userAccessToken)}");
            return userAccessToken;
        }

        public void ClearExpiredTokensFromCache()
        {
            foreach (var (key, userToken) in AccessTokens)
            {
                // only remove expired tokens after 6 hours
                if (userToken.Expiration >= DateTime.UtcNow.AddHours(-6)) continue;

                _ = AccessTokens.Remove(key, out _);
                _logger.LogDebug($"Removed expired token {userToken.AccessToken} from cache.");
            }
        }

        public async Task PopulateCache(IEnumerable<UserAccessToken> tokens)
        {
            try
            {
                _logger.LogDebug("populating...");
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

            _logger.LogDebug($"Cache populated: {JsonUtility.PrettyPrint(AccessTokens)}");
        }

        public void RemoveAccessToken(string accessToken)
        {
            _ = AccessTokens.Remove(accessToken, out _);

            _logger.LogDebug($"Removed expired token {accessToken} from cache.");
        }


        public static void SetAccessToken(UserAccessToken token)
        {
            _ = AccessTokens.AddOrUpdate(token.AccessToken, token, (_, b) =>
            {
                if (b == null) throw new ArgumentNullException(nameof(b));
                b = token;
                return b;
            });
        }
    }
}
