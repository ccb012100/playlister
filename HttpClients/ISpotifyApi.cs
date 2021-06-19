using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.SpotifyApi;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyApi
    {
        /// <summary>
        /// Get detailed profile information about the current user (including the current user’s username).
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>The User who was assigned the provided Access Token.</returns>
        [Get("/me")]
        Task<PrivateUserObject> GetCurrentUser(CancellationToken ct);

        /// <summary>
        /// Get a list of the playlists owned or followed by the current Spotify user.
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="limit">The maximum number of playlists to return. Default: <c>20</c>. Minimum: <c>1</c>. Maximum: <c>50</c></param>
        /// <param name="offset">‘The index of the first playlist to return. Default: <c>0</c> (the first object). Maximum offset: <c>100.000</c>. Use with limit to get the next set of playlists.’</param>
        /// <returns></returns>
        [Get("/me/playlists?market=from_token")]
        Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylists(CancellationToken ct,
            int? limit = 50, int? offset = null);
    }
}
