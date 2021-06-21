using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Requests;
using Playlister.HttpClients;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the current user's playlists to the db.
    /// </summary>
    public class UpsertCurrentUserPlaylistsHandler : IRequestHandler<CurrentUserUpsertPlaylistsRequest, Unit>
    {
        private readonly IMediator _mediator;
        private readonly SpotifyApiService _api;
        private readonly ILogger<UpsertCurrentUserPlaylistsHandler> _logger;

        public UpsertCurrentUserPlaylistsHandler(IMediator mediator, SpotifyApiService api,
            ILogger<UpsertCurrentUserPlaylistsHandler> logger)
        {
            _mediator = mediator;
            _api = api;
            _logger = logger;
        }

        public async Task<Unit> Handle(CurrentUserUpsertPlaylistsRequest request, CancellationToken ct)
        {
            PagingObject<SimplifiedPlaylistObject> listPage = await _api.GetCurrentUserPlaylists(ct);

            await _mediator.Send(new UpsertPlaylistsRequest(listPage.Items), ct);

            while (listPage.Next is not null)
            {
                listPage = await _api.GetCurrentUserPlaylists(listPage.Next, ct);
                await _mediator.Send(new UpsertPlaylistsRequest(listPage.Items), ct);
            }

            return new Unit();
        }
    }
}
