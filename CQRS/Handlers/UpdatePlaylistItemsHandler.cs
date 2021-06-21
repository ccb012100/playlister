using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Requests;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Repositories;
using Playlister.Utilities;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Update the list of Tracks for the specified playlist.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistItemsHandler : IRequestHandler<UpdatePlaylistItemsRequest, Unit>
    {
        private readonly IPlaylistTrackRepository _trackRepository;
        private readonly SpotifyApiService _api;

        public UpdatePlaylistItemsHandler(SpotifyApiService api, IPlaylistTrackRepository trackRepository)
        {
            _trackRepository = trackRepository;
            _api = api;
        }

        public async Task<Unit> Handle(UpdatePlaylistItemsRequest request, CancellationToken ct)
        {
            // get first page
            PagingObject<PlaylistItem> page =
                await _api.GetPlaylistItems(request.Playlist.Id, request.Offset, request.Limit, ct);

            await _trackRepository.Upsert(request.Playlist, page.Items, ct);

            // keep getting next page until Page.Next is null
            while (page.Next is not null)
            {
                page = await _api.GetPlaylistItems(page.Next!, ct);

                await _trackRepository.Upsert(request.Playlist, page.Items, ct);
            }

            await PageObjectProcessor.ProcessPages(
                async token =>
                    await _api.GetPlaylistItems(request.Playlist.Id, request.Offset, request.Limit, token),
                async (next, token) => await _api.GetPlaylistItems(next, token),
                async (tracks, token) =>
                    await _trackRepository.Upsert(request.Playlist, tracks, token),
                ct);

            return new Unit();
        }
    }
}
