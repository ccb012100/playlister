using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    public class GetCurrentUserPlaylistsHandler
        : IRequestHandler<GetCurrentUserPlaylistsCommand, IEnumerable<Playlist>>
    {
        private readonly IPlaylistService _playlistService;

        public GetCurrentUserPlaylistsHandler(IPlaylistService playlistService) =>
            _playlistService = playlistService;

        public async Task<IEnumerable<Playlist>> Handle(
            GetCurrentUserPlaylistsCommand command,
            CancellationToken ct
        ) => await _playlistService.GetCurrentUserPlaylistsAsync(command.AccessToken, ct);
    }
}
