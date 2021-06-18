using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Requests;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistHandler : IRequestHandler<UpdatePlaylistRequest, Unit>
    {
        private readonly ISpotifyApi _api;
        private readonly ILogger<UpdatePlaylistHandler> _logger;

        public UpdatePlaylistHandler(ISpotifyApi api, ILogger<UpdatePlaylistHandler> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePlaylistRequest request, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            // TODO: if playlist SnapshotId is the same as in the DB, skip
            // var playlist = get Playlist();

            int offset = 0, itemsProcessed = 0;
            const int limit = 20;

            // page through playlist tracks
            PagingObject<PlaylistItem> playlistItems =
                await _api.GetPlaylistItems(request.PlaylistId, offset, limit, cancellationToken);

            do
            {
                foreach (var playlistItem in playlistItems.Items)
                {
                    /*
                     * TODO:
                     * - if entry exists in DB with matching (track_id, playlist_id), update snapshot_id
                     * - else add to DB
                     */
                    _logger.LogInformation(
                        $"{playlistItem.Track.Name} by {string.Join(", ", playlistItem.Track.Artists.Select(a => a.Name))}");
                    itemsProcessed++;
                }

                if (playlistItems.Next == null)
                {
                    continue;
                }

                offset += playlistItems.Items.Count;
                playlistItems = await _api.GetPlaylistItems(request.PlaylistId, offset, limit, cancellationToken);
            } while (playlistItems.Next != null);

            // TODO: figure out why there seems to be an off-by-one error for each page of data
            timer.Stop();
            _logger.LogInformation($"Getting the {itemsProcessed} playlist items took {timer.Elapsed.TotalSeconds}");
            return new Unit();
        }
    }
}
