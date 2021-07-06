using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the current user's playlists to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdateCurrentUserPlaylistsHandler : IRequestHandler<UpdateCurrentUserPlaylistsCommand, int>
    {
        private readonly ISpotifyApiService _api;
        private readonly IPlaylistService _playlistService;
        private readonly ILogger<UpdateCurrentUserPlaylistsHandler> _logger;

        public UpdateCurrentUserPlaylistsHandler(ISpotifyApiService api, IPlaylistService playlistService,
            ILogger<UpdateCurrentUserPlaylistsHandler> logger)
        {
            _api = api;
            _playlistService = playlistService;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateCurrentUserPlaylistsCommand command, CancellationToken ct)
        {
            int total = 0;

            PagingObject<SimplifiedPlaylistObject> page = await _api.GetCurrentUserPlaylists(command.AccessToken, ct);

            await _playlistService.UpdatePlaylists(command.AccessToken, page.Items, ct);
            total += page.Items.Count();
            while (page.Next is not null)
            {
                page = await _api.GetCurrentUserPlaylists(command.AccessToken, page.Next, ct);
                await _playlistService.UpdatePlaylists(command.AccessToken, page.Items, ct);
                total += page.Items.Count();
            }

            _logger.LogInformation($"Processed a total of `{total}` playlists.");
            return total;
        }
    }
}
