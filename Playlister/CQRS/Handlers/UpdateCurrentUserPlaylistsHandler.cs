using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the current user's playlists to the db.
    /// </summary>

    public class UpdateCurrentUserPlaylistsHandler
        : IRequestHandler<UpdateCurrentUserPlaylistsCommand, int>
    {
        private readonly IPlaylistService _playlistService;
        private readonly ILogger<UpdateCurrentUserPlaylistsHandler> _logger;

        public UpdateCurrentUserPlaylistsHandler(IPlaylistService playlistService, ILogger<UpdateCurrentUserPlaylistsHandler> logger)
        {
            _playlistService = playlistService;
            _logger = logger;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns>Number of playlists handled.</returns>
        public async Task<int> Handle(
            UpdateCurrentUserPlaylistsCommand command,
            CancellationToken ct
        )
        {
            var sw = Stopwatch.StartNew();

            ImmutableArray<Playlist> playlists = (
                await _playlistService.GetCurrentUserPlaylistsAsync(command.AccessToken, ct)!
            ).ToImmutableArray();

            double getPlaylist_elapsedMs = sw.Elapsed.TotalMilliseconds;
            _logger.LogInformation(
                "\n=> {DateTime}: It took {Elapsed}ms to get the current user's playlists.\n",
                DateTime.Now.ToLocalTime(),
                getPlaylist_elapsedMs
            );

            await _playlistService.UpdatePlaylistsAsync(command.AccessToken, playlists, ct);

            sw.Stop();

            double updatePlaylist_elapsedMs = sw.Elapsed.TotalMilliseconds - getPlaylist_elapsedMs;

            _logger.LogInformation("\n=> {DateTime}: It took {Elapsed}ms to update the current user's playlists", DateTime.Now.ToLocalTime(), updatePlaylist_elapsedMs.ToString("N5"));

            return playlists.Length;
        }
    }
}
