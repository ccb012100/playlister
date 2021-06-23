using System.Collections.Generic;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;

namespace Playlister.Services
{
    public interface ICacheService
    {
        UserAccessToken Set(SpotifyAccessToken token);
        UserAccessToken? Get(string accessToken);
        void ClearExpiredTokensFromCache();
        void PopulateCache(IEnumerable<UserAccessToken> tokens);
        void RemoveAccessToken(string accessToken);
    }
}
