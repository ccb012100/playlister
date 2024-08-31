using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

/// <summary>
///     Sync the Playlists in the command to the db, regardless of whether it's up-to-date or not.
/// </summary>
public class ForceSyncPlaylistsHandler : IRequestHandler<SyncPlaylistCommand, Unit>
{
    private readonly IPlaylistService _playlistService;

    public ForceSyncPlaylistsHandler(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    public async Task<Unit> Handle(SyncPlaylistCommand command, CancellationToken ct)
    {
        await Task.Run(() => _playlistService.ForceSyncPlaylistAsync(command.AccessToken, command.PlaylistId, ct), ct);

        return new Unit();
    }
}
