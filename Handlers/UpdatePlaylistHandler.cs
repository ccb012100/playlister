using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Requests;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistHandler : IRequestHandler<UpdatePlaylistRequest, Unit>
    {
        private readonly SpotifyApiService _spotifyApiService;
        private readonly ILogger<UpdatePlaylistHandler> _logger;

        public UpdatePlaylistHandler(SpotifyApiService spotifyApiService, ILogger<UpdatePlaylistHandler> logger)
        {
            _spotifyApiService = spotifyApiService;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePlaylistRequest request, CancellationToken ct)
        {
            int offset = request.Offset ?? 0, itemsProcessed = 0;
            int limit = request.Limit ?? 20;

            var timer = new Stopwatch();
            timer.Start();

            // TODO: if playlist SnapshotId is the same as in the DB, skip
            // var playlist = get Playlist();

            // page through playlist tracks
            PagingObject<PlaylistItem> playlistItems =
                await _spotifyApiService.GetPlaylistItems(request.PlaylistId, offset, limit, ct);

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
                     * - else add to DB
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
