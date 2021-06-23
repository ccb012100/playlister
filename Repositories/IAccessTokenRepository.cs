using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;

namespace Playlister.Repositories
{
    public interface IAccessTokenRepository
    {
        Task<UserAccessToken> AddToken(SpotifyAccessToken spotifyToken);
        UserAccessToken? Get(string accessToken);
        void RemoveAccessToken(string accessToken);
    }
}
