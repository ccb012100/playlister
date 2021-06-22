using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Repositories;

namespace Playlister.Services
{
    public interface ICacheService
    {
        UserAccessToken SetAccessToken(SpotifyAccessToken token);
        UserAccessToken? GetToken(string accessToken);
        void RemoveAccessToken(string accessToken);
        void ClearExpiredTokensFromCache();
        Task PopulateCacheFromDatabase(IAccessTokenRepository repository);
    }
}
