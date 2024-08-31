
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class GetCurrentUserPlaylistsHandler
    : ICommandHandler
{
    private readonly IPlaylistService _playlistService;

    public GetCurrentUserPlaylistsHandler(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    public async Task<IEnumerable<Playlist>> Handle(
        GetCurrentUserPlaylistsCommand command,
        CancellationToken ct = default
    )
    {
        return await _playlistService.GetUserPlaylistsAsync(command.AccessToken, ct);
    }
}
