using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Requests;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the Playlists in request to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpsertPlaylistsHandler : IRequestHandler<UpdatePlaylistsRequest, Unit>
    {
        private readonly IMediator _mediator;

        public UpsertPlaylistsHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdatePlaylistsRequest request, CancellationToken ct)
        {
            // TODO: run this as a background job, and call UpdatePlaylistHandler for each Playlist
            foreach (SimplifiedPlaylistObject playlist in request.Playlists)
            {
                await _mediator.Send(new UpdatePlaylistItemsRequest(playlist.Id), ct);
            }

            return new Unit();
        }
    }
}
