using System.Collections.Generic;
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
    // ReSharper disable once UnusedType.Global
    public class CurrentUserPlaylistsHandler : IRequestHandler<CurrentUserPlaylistsRequest,
        IEnumerable<Playlist>>
    {
        private readonly SpotifyApiService _service;
        private readonly ILogger<CurrentUserPlaylistsHandler> _logger;

        public CurrentUserPlaylistsHandler(SpotifyApiService service, ILogger<CurrentUserPlaylistsHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IEnumerable<Playlist>> Handle(CurrentUserPlaylistsRequest request,
            CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();

            PagingObject<SimplifiedPlaylistObject> page = await _service.GetCurrentUserPlaylists(ct);
            List<Playlist> lists = page.Items.Select(i => i.ToPlaylist()).ToList();

            while (page.Next is not null)
            {
                page = await _service.GetCurrentUserPlaylists(page.Next, ct);

                lists.AddRange(page.Items.Select(i => i.ToPlaylist()));
            }

            sw.Stop();
            _logger.LogInformation($"It took {sw.ElapsedMilliseconds}ms to convert {lists.Count} items.");

            return lists;
        }
    }
}
