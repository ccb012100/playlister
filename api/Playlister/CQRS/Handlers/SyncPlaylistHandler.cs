using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

/// <summary>
///     Sync the Playlist in the command to the DB.
/// </summary>
public class SyncPlaylistHandler : IRequestHandler<SyncPlaylistCommand, Unit>
{
    private readonly IPlaylistService _playlistService;

    public SyncPlaylistHandler(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    /// <summary>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <returns>Number of playlists handled</returns>
    public async Task<Unit> Handle(SyncPlaylistCommand command, CancellationToken ct)
    {
        await Task.Run(
            () => _playlistService.SyncPlaylistAsync(
                command.AccessToken,
                command.PlaylistId,
                ct
            ),
            ct
        );

        return new Unit();
    }
}
