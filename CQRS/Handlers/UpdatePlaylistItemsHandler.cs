using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Requests;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Update the list of Tracks for the specified playlist.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistItemsHandler : IRequestHandler<UpdatePlaylistItemsRequest, Unit>
    {
        private readonly SpotifyApiService _spotifyApiService;
        private readonly ILogger<UpdatePlaylistItemsHandler> _logger;

        public UpdatePlaylistItemsHandler(SpotifyApiService spotifyApiService,
            ILogger<UpdatePlaylistItemsHandler> logger)
        {
            _spotifyApiService = spotifyApiService;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePlaylistItemsRequest itemsRequest, CancellationToken ct)
        {
            int offset = itemsRequest.Offset, itemsProcessed = 0;
            int limit = itemsRequest.Limit;

            var timer = new Stopwatch();
            timer.Start();

            // TODO: get playlist entry from DB
            // TODO: if playlist SnapshotId is the same as in the DB, skip

            // page through playlist tracks
            PagingObject<PlaylistItem> playlistItems =
                await _spotifyApiService.GetPlaylistItems(itemsRequest.PlaylistId, offset, limit, ct);

            do
            {
                if (itemsProcessed > 0)
                {
                    playlistItems = await _spotifyApiService.GetPlaylistItems(playlistItems.Next!, ct);
                }

                foreach (var playlistItem in playlistItems.Items)
                {
                    /*
                     * TODO:
                     * - if entry exists in DB with matching (track_id, playlist_id), update snapshot_id
                     * - else add to DB (do through sqlite upsert)
                     */
                    itemsProcessed++;

                    _logger.LogInformation(
                        $"{itemsProcessed} - {playlistItem.Track.Name} by {string.Join(", ", playlistItem.Track.Artists.Select(a => a.Name))}");
                }
            } while (playlistItems.Next != null);

            timer.Stop();
            _logger.LogInformation($"Getting the {itemsProcessed} playlist items took {timer.Elapsed.TotalSeconds}");
            return new Unit();
        }
    }
}
