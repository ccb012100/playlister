using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Requests;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Repositories;
using Playlister.Services;

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
            Playlist? playlist = _playlistRepository.Get(request.PlaylistId);
            SimplifiedPlaylistObject playlistObject = await _api.GetPlaylist(request.PlaylistId, ct);

            // return without processing if the DB version matches the request version
            if (playlist is not null && playlist.SnapshotId == playlistObject.SnapshotId)
            {
                _logger.LogDebug(
                    $"Request id `{request.PlaylistId}` (playlist name `{playlist.Name}`) hasn't changed since the last update.");
                return new Unit();
            }

            // get first page of playlist items
            PagingObject<PlaylistItem> page =
                await _api.GetPlaylistItems(request.PlaylistId, request.Offset, request.Limit, ct);

            // We want to get all items so that they can be inserted into the repository in a single Transaction
            List<PlaylistItem> allItems = page.Items.ToList();

            while (page.Next is not null)
            {
                page = await _api.GetPlaylistItems(page.Next, ct);
                allItems.AddRange(page.Items);
            }

            await _trackRepository.Upsert(playlistObject.ToPlaylist(), allItems, ct);
            // refresh cache because the playlist was updated through the IPlaylistTrackRepository and not IPlaylistRepository
            await _playlistRepository.RefreshCache();

            return new Unit();
        }
    }
}
