using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Playlister.Models;

namespace Playlister.Utilities
{
    public static class TokenUtility
    {
        /// <summary>
        /// Create and cache a new User Access Token
        /// </summary>
        /// <param name="spotifyToken"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static UserAccessToken CreateUserAccessToken(SpotifyAccessToken spotifyToken, IMemoryCache cache,
            ILogger logger)
        {
            var userToken = new UserAccessToken
            {
                AccessToken = spotifyToken.AccessToken,
                Expiration = DateTime.Now.AddSeconds(spotifyToken.ExpiresIn),
                RefreshToken = spotifyToken.RefreshToken,
                Scopes = spotifyToken.Scope.Split(' ')
            };

            // use AccessToken as cache key
            cache.Set(spotifyToken.AccessToken, userToken, TimeSpan.FromSeconds(spotifyToken.ExpiresIn));
            logger.LogDebug($"Added access token key={spotifyToken.AccessToken} to cache.");

            return userToken;
        }
    }
}
