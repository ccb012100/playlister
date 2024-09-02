using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

/// <summary>
///     Sync the Playlist in the command to the db, regardless of whether it's up-to-date or not.
/// </summary>
public class ForceSyncPlaylistHandler : ICommandHandler
{
    private readonly IPlaylistService _playlistService;

    public ForceSyncPlaylistHandler( IPlaylistService playlistService )
    {
        _playlistService = playlistService;
    }

    public async Task<Unit> Handle( ForceSyncPlaylistCommand command, CancellationToken ct = default )
    {
        await Task.Run( () => _playlistService.ForceSyncPlaylistAsync( command.AccessToken, command.PlaylistId, ct ), ct );

        return new Unit();
    }
}
