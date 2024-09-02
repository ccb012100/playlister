using System.Collections.Immutable;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

/// <summary>
///     Add or Update the current user's playlists to the db.
/// </summary>
public class SyncCurrentUserPlaylistsHandler : ICommandHandler
{
    private readonly IPlaylistService _playlistService;

    public SyncCurrentUserPlaylistsHandler( IPlaylistService playlistService )
    {
        _playlistService = playlistService;
    }

    /// <summary>
    ///     Update Current user's playlists
    /// </summary>
    /// <returns>Number of playlists Updated.</returns>
    public async Task<(int total, int updated, int deleted)> Handle( SyncCurrentUserPlaylistsCommand command, CancellationToken ct = default )
    {
        ImmutableArray<Playlist> playlists = await _playlistService.GetUserPlaylistsAsync( command.AccessToken, ct );

        int updated = await _playlistService.SyncPlaylistsAsync( command.AccessToken, playlists, ct );
        int deleted = await _playlistService.DeleteOrphanedPlaylistTracksAsync( ct );

        return (total: playlists.Length, updated, deleted);
    }
}
