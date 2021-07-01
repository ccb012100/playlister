using System;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Services
{
    public interface ISpotifyApiService
    {
        /// <summary>
        /// Get the tracks for specified <param name="playlistId"></param>.
        /// </summary>
        /// <param name="playlistId">Spotify Id of the playlist</param>
        /// <param name="offset"></param>
        /// <param name="limit">Number of items to return</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<PagingObject<PlaylistItem>> GetPlaylistTracks(string playlistId, int? offset, int? limit,
            CancellationToken ct);

        Task<PagingObject<PlaylistItem>> GetPlaylistTracks(Uri next, CancellationToken ct);

        /// <summary>
        /// Get a list of the playlists owned or followed by the current Spotify user.
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="offset">‘The index of the first playlist to return. Default: <c>0</c> (the first object). Maximum offset: <c>100.000</c>. Use with limit to get the next set of playlists.’</param>
        /// <param name="limit">The maximum number of playlists to return. Default: <c>20</c>. Minimum: <c>1</c>. Maximum: <c>50</c></param>
        /// <returns></returns>
        Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylists(CancellationToken ct,
            int? offset = null, int? limit = 50);

        /// <summary>
        /// Get the current user's playlists.
        /// </summary>
        /// <param name="next">URI of the next page of playlists</param>
        /// <param name="ct"></param>
        /// <returns>A <c>PagingObject</c> containing the playlists requested in the <paramref name="next"/> Uri parameter</returns>
        Task<PagingObject<SimplifiedPlaylistObject>>
            GetCurrentUserPlaylists(Uri next, CancellationToken ct);

        Task<SimplifiedPlaylistObject> GetPlaylist(string playlistId, CancellationToken ct);
    }
}