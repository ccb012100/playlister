using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Update the list of Tracks for the specified playlist.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistHandler : IRequestHandler<UpdatePlaylistCommand, Unit>
    {
        private readonly ILogger<UpdatePlaylistHandler> _logger;
        private readonly ISpotifyApiService _api;
        private readonly IPlaylistService _playlistService;

        public UpdatePlaylistHandler(ISpotifyApiService api, IPlaylistService playlistService,
            ILogger<UpdatePlaylistHandler> logger)
        {
            _logger = logger;
            _api = api;
            _playlistService = playlistService;
        }

        public async Task<Unit> Handle(UpdatePlaylistCommand command, CancellationToken ct)
        {
            Playlist? playlist = _playlistService.GetPlaylist(command.PlaylistId);
            SimplifiedPlaylistObject playlistObject = await _api.GetPlaylist(command.PlaylistId, ct);

            // return without processing if the DB version matches the command version
            if (playlist is not null && playlist.SnapshotId == playlistObject.SnapshotId)
            {
                _logger.LogDebug(
                    $"Request id `{command.PlaylistId}` (playlist name `{playlist.Name}`) hasn't changed since the last update.");
                return new Unit();
            }

            // get first page of playlist items
            PagingObject<PlaylistItem> page =
                await _api.GetPlaylistTracks(command.PlaylistId, command.Offset, command.Limit, ct);

            // We want to get all items so that they can be inserted into the repository in a single Transaction
            List<PlaylistItem> allItems = page.Items.ToList();

            while (page.Next is not null)
            {
                page = await _api.GetPlaylistTracks(page.Next, ct);
                allItems.AddRange(page.Items);
            }

            await _playlistService.UpdatePlaylist(playlistObject.ToPlaylist(), allItems, ct);

            return new Unit();
        }
    }
}
