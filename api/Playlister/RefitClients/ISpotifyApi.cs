using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Refit;

namespace Playlister.RefitClients;

public interface ISpotifyApi
{
    /// <summary>
    ///     Get detailed profile information about the current user (including the current user’s username).
    /// </summary>
    /// <param name="token"></param>
    /// <param name="ct"></param>
    /// <returns>The User who was assigned the provided Access Token.</returns>
    [Get("/me")]
    Task<PrivateUserObject> GetCurrentUserAsync([Authorize] string token, CancellationToken ct);

    /// <summary>
    ///     Get playlist.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="playlistId">Spotify Id of the playlist</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Get("/playlists/{playlistId}")]
    Task<SimplifiedPlaylistObject> GetPlaylistAsync([Authorize] string token, string playlistId,
        CancellationToken ct);

    /// <summary>
    ///     Get a list of the playlists owned or followed by the current Spotify user.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="offset">The index of the first item to return.</param>
    /// <param name="limit">The maximum number of items to return. Default: <c>20</c>. Minimum: <c>1</c>. Maximum: <c>50</c>.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Get("/me/playlists?market=from_token")]
    Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync([Authorize] string token, int? offset,
        int? limit, CancellationToken ct);

    /// <summary>
    ///     Get full details of the items of a playlist owned by a Spotify user.
    ///     Applying the fields query
    ///     <c>fields=limit,next,previous,offset,limit,total,href,items(added_at,track(id,track_number,disc_number,duration_ms,name,artists(id,name),album(name,id,release_date,total_tracks,album_type,artists(id,name))))</c>
    /// </summary>
    /// <param name="token"></param>
    /// <param name="playlistId">Playlist's Spotify Id</param>
    /// <param name="offset">The index of the first item to return. Default: <c>0</c> (the first object).</param>
    /// <param name="limit">The maximum number of items to return. Default: <c>100</c>. Minimum: <c>100</c>. Maximum: <c>100</c>.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Get(
        "/playlists/{playlistId}/tracks?market=from_token&" +
        "fields=limit,next,previous,offset,limit,total,href,items(added_at,track(id,track_number,disc_number,duration_ms,name,artists(id,name),album(name,id,release_date,total_tracks,album_type,artists(id,name))))")]
    Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync([Authorize] string token, string playlistId,
        int? offset, int? limit, CancellationToken ct);
}
