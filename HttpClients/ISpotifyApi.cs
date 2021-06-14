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
        /// <param name="cancellationToken"></param>
        /// <returns>The User who was assigned the provided Access Token.</returns>
        [Get("/me")]
        Task<PrivateUserObject> GetCurrentUser(CancellationToken cancellationToken);

        /// <summary>
        /// Get a list of the playlists owned or followed by the current Spotify user.
        /// </summary>
        /// <param name="requestParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Get("/me/playlists")]
        Task<PagingObject<PlaylistObject>> GetCurrentUserPlaylists(CurrentUserPlaylistsRequest requestParams,
            CancellationToken cancellationToken);
    }
}
