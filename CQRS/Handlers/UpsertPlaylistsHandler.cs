using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the Playlists in command to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpsertPlaylistsHandler : IRequestHandler<UpdatePlaylistsCommand, Unit>
    {
        private readonly IMediator _mediator;

        public UpsertPlaylistsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdatePlaylistsCommand command, CancellationToken ct)
        {
            // TODO: run this as a background job, and call UpdatePlaylistHandler for each Playlist
            foreach (SimplifiedPlaylistObject playlist in command.Playlists)
            {
                await _mediator.Send(new UpdatePlaylistItemsCommand(playlist.Id), ct);
            }

            return new Unit();
        }
    }
}
