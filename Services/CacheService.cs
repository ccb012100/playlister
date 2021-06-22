using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Repositories;

namespace Playlister.Services
{
    public class CacheService : ICacheService
    {
        private static readonly ConcurrentDictionary<string, UserAccessToken> AccessTokens = new();

        public UserAccessToken SetAccessToken(SpotifyAccessToken token) =>
            AccessTokens.AddOrUpdate(token.AccessToken, token.ToUserAccessToken(), (_, b) =>
            {
                if (b == null) throw new ArgumentNullException(nameof(b));
                b = token.ToUserAccessToken();
                return b;
            });


        public UserAccessToken? GetToken(string accessToken)
        {
            _ = AccessTokens.TryGetValue(accessToken, out UserAccessToken? userAccessToken);
            return userAccessToken;
        }

        public void RemoveAccessToken(string accessToken) =>
            _ = AccessTokens.Remove(accessToken, out _);

        public void ClearExpiredTokensFromCache()
        {
            foreach (var (key, userToken) in AccessTokens)
            {
                // remove expired tokens after 6 hours
                if (userToken.Expiration < DateTime.UtcNow.AddHours(-6))
                {
                    _ = AccessTokens.Remove(key, out _);
                }
            }
        }

        private void SetAccessToken(UserAccessToken token)
        {
            _ = AccessTokens.AddOrUpdate(token.AccessToken, token, (_, b) =>
            {
                if (b == null) throw new ArgumentNullException(nameof(b));
                b = token;
                return b;
            });
        }

        public async Task PopulateCacheFromDatabase(IAccessTokenRepository repository)
        {
            IEnumerable<UserAccessToken> tokens = await repository.GetAll();

            foreach (UserAccessToken token in tokens)
            {
                SetAccessToken(token);
            }
        }
    }
}
