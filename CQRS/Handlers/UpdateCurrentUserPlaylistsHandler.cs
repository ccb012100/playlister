using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;
using Playlister.Utilities;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the current user's playlists to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdateCurrentUserPlaylistsHandler : IRequestHandler<CurrentUserUpdatePlaylistsCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly SpotifyApiService _api;

        public UpdateCurrentUserPlaylistsHandler(IMediator mediator, SpotifyApiService api)
        {
            _mediator = mediator;
            _api = api;
        }

        public async Task<Unit> Handle(CurrentUserUpdatePlaylistsCommand command, CancellationToken ct)
        {
            await PageObjectProcessor.ProcessPages(
                async token => await _api.GetCurrentUserPlaylists(token),
                async (next, token) => await _api.GetCurrentUserPlaylists(next, token),
                async (playlistObjects, token) =>
                    await _mediator.Send(new UpdatePlaylistsCommand(playlistObjects), token),
                ct);

            return new Unit();
        }
    }
}
