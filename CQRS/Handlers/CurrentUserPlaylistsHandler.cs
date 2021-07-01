using System.Collections.Generic;
using System.Diagnostics;
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
    // ReSharper disable once UnusedType.Global
    public class CurrentUserPlaylistsHandler : IRequestHandler<CurrentUserPlaylistsCommand,
        IEnumerable<Playlist>>
    {
        private readonly ISpotifyApiService _service;
        private readonly ILogger<CurrentUserPlaylistsHandler> _logger;

        public CurrentUserPlaylistsHandler(ISpotifyApiService service, ILogger<CurrentUserPlaylistsHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IEnumerable<Playlist>> Handle(CurrentUserPlaylistsCommand command,
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
