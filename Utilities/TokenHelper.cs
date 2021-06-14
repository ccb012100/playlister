using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;

namespace Playlister.Utilities
{
    public static class TokenUtility
    {
        /// <summary>
        /// Create and cache a new User Access Token
        /// </summary>
        /// <param name="info"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static UserAccessInfo CreateUserAccessToken(AccessInfo info, IMemoryCache cache,
            ILogger logger)
        {
            var userToken = new UserAccessInfo
            {
                AccessToken = info.AccessToken,
                Expiration = DateTime.Now.AddSeconds(info.ExpiresIn),
                RefreshToken = info.RefreshToken,
                Scopes = info.Scope.Split(' ')
            };

            // use AccessToken as cache key
            cache.Set(info.AccessToken, userToken, TimeSpan.FromSeconds(info.ExpiresIn));
            logger.LogDebug($"Added access token key={info.AccessToken} to cache.");

            return userToken;
        }
    }
}
