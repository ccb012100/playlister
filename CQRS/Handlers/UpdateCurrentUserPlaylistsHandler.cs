using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Commands;
using Playlister.Services;
using Playlister.Utilities;

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
            int total = await PageObjectProcessor.ProcessPages(
                async token => await _api.GetCurrentUserPlaylists(command.AccessToken, token),
                async (next, token) => await _api.GetCurrentUserPlaylists(command.AccessToken, next, token),
                async (playlistObjects, token) =>
                    await _playlistService.UpdatePlaylists(command.AccessToken, playlistObjects, token), ct);
            // TODO: figure out why this logger statement never seems to run
            _logger.LogInformation($"PageObjectProcessor returned a total of `{total}`.");
            return total;
        }
    }
}
