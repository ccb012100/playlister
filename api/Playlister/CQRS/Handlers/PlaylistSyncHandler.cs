using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class PlaylistSyncHandler
{
    private readonly IPlaylistService _playlistService;

    public PlaylistSyncHandler( IPlaylistService playlistService )
    {
        _playlistService = playlistService;
    }

    /// <summary>
    ///     Sync multiple Playlists to the database.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <returns>
    ///     Number of playlists handled
    /// </returns>
    /// <remarks>
    ///     Syncing is a no-op for any Playlists that are already up-to-date.
    /// </remarks>
    public async Task<int> SyncMultiple( SyncPlaylistsCommand command, CancellationToken ct = default )
    {
        return await Task.Run(
            () => _playlistService.SyncPlaylistsAsync(
                command.AccessToken,
                command.Playlists.Select( p => p.ToPlaylist() ),
                ct
            ),
            ct
        );
    }

    /// <summary>
    ///     Sync a single Playlist to the database.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <remarks>
    ///     If the Playlist is up-to-date, this is a no-op.
    /// </remarks>
    public async Task SyncSingle( SyncPlaylistCommand command, CancellationToken ct = default )
    {
        await Task.Run(
            () => _playlistService.SyncPlaylistAsync(
                command.AccessToken,
                command.PlaylistId,
                ct
            ),
            ct
        );
    }

    /// <summary>
    ///     Sync all Playlists owned by the current User to the database.
    /// </summary>
    /// <returns>
    ///     Number of playlists Updated.
    /// </returns>
    /// <remarks>
    ///     Syncing is a no-op for any Playlists that are already up-to-date.
    /// </remarks>
    public async Task<(int total, int updated, int deleted)> SyncAllForCurrentUser(
        SyncCurrentUserPlaylistsCommand command,
        CancellationToken ct = default
    )
    {
        ImmutableArray<Playlist> playlists = await _playlistService.GetUserPlaylistsAsync( command.AccessToken, ct );

        int updated = await _playlistService.SyncPlaylistsAsync( command.AccessToken, playlists, ct );
        int deleted = await _playlistService.DeleteOrphanedPlaylistTracksAsync( ct );

        return (total: playlists.Length, updated, deleted);
    }

    /// <summary>
    ///     Sync the Playlist in the command to the database, regardless of whether it's up-to-date or not.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <remarks>
    ///     This always performs a fresh sync of the Playlist.
    /// </remarks>
    public async Task ForceSync( ForceSyncPlaylistCommand command, CancellationToken ct = default )
    {
        await Task.Run( () => _playlistService.ForceSyncPlaylistAsync( command.AccessToken, command.PlaylistId, ct ), ct );
    }
}
