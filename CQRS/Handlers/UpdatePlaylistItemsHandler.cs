using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Requests;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Repositories;
using Playlister.Services;
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
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ILogger<UpdatePlaylistItemsHandler> _logger;
        private readonly SpotifyApiService _api;

        public UpdatePlaylistItemsHandler(SpotifyApiService api, IPlaylistTrackRepository trackRepository,
            IPlaylistRepository playlistRepository, ILogger<UpdatePlaylistItemsHandler> logger)
        {
            _trackRepository = trackRepository;
            _playlistRepository = playlistRepository;
            _logger = logger;
            _api = api;
        }

        public async Task<Unit> Handle(UpdatePlaylistItemsRequest request, CancellationToken ct)
        {
            Playlist? playlist = _playlistRepository.Get(request.Playlist.Id);

            // return without processing if the DB version matches the request version
            if (playlist is not null && playlist.SnapshotId == request.Playlist.SnapshotId)
            {
                _logger.LogDebug(
                    $"Request id={request.Playlist.Id}, snapshot id={request.Playlist.SnapshotId} matches current data.");
                return new Unit();
            }

            // insert/update playlist
            SimplifiedPlaylistObject playlistObject = await _api.GetPlaylist(request.Playlist.Id, ct);
            await _playlistRepository.Upsert(playlistObject);

            // get first page of playlist items
            PagingObject<PlaylistItem> page =
                await _api.GetPlaylistItems(request.Playlist.Id, request.Offset, request.Limit, ct);

            await _trackRepository.Upsert(request.Playlist, page.Items, ct);

            // process the rest of the pages
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
