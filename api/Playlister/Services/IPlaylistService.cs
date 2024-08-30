using System.Collections.Immutable;
using Playlister.CQRS.Commands;
using Playlister.Models;

namespace Playlister.Services;

public interface IPlaylistService
{
    /// <summary>
    ///     Update the playlist specified in the <paramref name="command"></paramref> parameter.
    /// </summary>
    Task UpdatePlaylistAsync(UpdatePlaylistCommand command, CancellationToken ct);

    /// <summary>
    ///     Update the playlists provided.<br /><br />
    ///     <b>Note:</b> The items in <paramref name="playlists" /> are directly compared to the versions in the database,
    ///     so the caller should be providing current versions retrieved from Spotify's API.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="playlists">
    ///     The playlists to update. These are directly compared to the versions in the database, so the caller should be providing
    ///     current versions retrieved from Spotify's API.
    /// </param>
    /// <param name="ct"></param>
    /// <returns>The number of playlists updated.</returns>
    Task<int> UpdatePlaylistsAsync(string accessToken, IEnumerable<Playlist> playlists, CancellationToken ct);

    /// <summary>
    ///     The full lists of playlists for the current user.
    /// </summary>
    Task<ImmutableArray<Playlist>> GetCurrentUserPlaylistsAsync(string accessToken, CancellationToken ct);

    /// <summary>
    ///     Sync the specified playlist.<br /><br />
    ///     This will do a full sync, even if the snapshot ID has not changed since the last update.
    /// </summary>
    Task SyncPlaylistAsync(string accessToken, string playlistId, CancellationToken ct);

    /// <summary>
    ///     Delete tracks without any Playlist associations
    /// </summary>
    /// <remarks>Total number of tracks deleted</remarks>
    Task<int> DeleteOrphanedPlaylistTracksAsync(CancellationToken ct);
}
