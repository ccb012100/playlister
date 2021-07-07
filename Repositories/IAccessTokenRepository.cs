using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;

namespace Playlister.Repositories
{
    public interface IAccessTokenRepository
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="spotifyToken"></param>
        /// <returns></returns>
        Task<UserAccessToken> AddTokenAsync(SpotifyAccessToken spotifyToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        UserAccessToken? Get(string accessToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="accessToken"></param>
        void RemoveAccessToken(string accessToken);
    }
}
