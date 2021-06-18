using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.SpotifyApi;
using Playlister.Requests;
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
        /// <param name="queryParams"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Get("/me/playlists?market=from_token")]
        Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylists(CurrentUserPlaylistsRequest queryParams,
            CancellationToken ct);
    }
}
