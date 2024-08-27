using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Services
{
    public interface ISpotifyApiService
    {
        /// <summary>
        ///     Get the tracks for specified
        ///     <param name="playlistId"></param>
        ///     .
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="playlistId">Spotify Id of the playlist</param>
        /// <param name="offset"></param>
        /// <param name="limit">Number of items to return</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync(string accessToken, string playlistId, int? offset,
            int? limit, CancellationToken ct);

        /// <summary>
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="next"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync(string accessToken, Uri next, CancellationToken ct);

        /// <summary>
        ///     Get a list of the playlists owned or followed by the current Spotify user.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="ct"></param>
        /// <param name="offset">
        ///     ‘The index of the first playlist to return. Default: <c>0</c> (the first object). Maximum offset: <c>100.000</c>. Use with limit
        ///     to get the next set of playlists.’
        /// </param>
        /// <param name="limit">The maximum number of playlists to return. Default: <c>20</c>. Minimum: <c>1</c>. Maximum: <c>50</c></param>
        /// <returns></returns>
        Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync(string accessToken, CancellationToken ct,
            int? offset = null, int? limit = 50);

        /// <summary>
        ///     Get the current user's playlists.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="next">URI of the next page of playlists</param>
        /// <param name="ct"></param>
        /// <returns>A <c>PagingObject</c> containing the playlists requested in the <paramref name="next" /> Uri parameter</returns>
        Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync(string accessToken, Uri next,
            CancellationToken ct);

        /// <summary>
        ///     Get Playlist
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="playlistId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<SimplifiedPlaylistObject> GetPlaylistAsync(string accessToken, string playlistId, CancellationToken ct);
    }
}
