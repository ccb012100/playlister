using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

/// <summary>
///     Sync the Playlists in the command to the DB.
/// </summary>
public class SyncPlaylistsHandler : IRequestHandler<SyncPlaylistsCommand, Unit>
{
    private readonly IPlaylistService _playlistService;

    public SyncPlaylistsHandler(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    /// <summary>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <returns>Number of playlists handled</returns>
    public async Task<Unit> Handle(SyncPlaylistsCommand command, CancellationToken ct)
    {
        await Task.Run(
            () => _playlistService.SyncPlaylistsAsync(
                command.AccessToken,
                command.Playlists.Select(p => p.ToPlaylist()),
                ct
            ),
            ct
        );

        return new Unit();
    }
}
