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
        /// <param name="queryParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Get("/me/playlists?market=from_token")]
        Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylists(CurrentUserPlaylistsRequest queryParams,
            CancellationToken cancellationToken);

        /// <summary>
        /// Get full details of the items of a playlist owned by a Spotify user.
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="offset">The index of the first item to return. Default: <c>0</c> (the first object).</param>
        /// <param name="limit">The maximum number of items to return. Default: <c>100</c>. Minimum: <c>100</c>. Maximum: <c>100</c>.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Get("/playlists/{playlistId}/tracks?market=from_token")]
        Task<PagingObject<PlaylistItemObject>> GetPlaylistItems(string playlistId, int? offset, int? limit,
            CancellationToken cancellationToken);
    }
}
