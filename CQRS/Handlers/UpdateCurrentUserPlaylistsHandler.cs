using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

        public UpdateCurrentUserPlaylistsHandler(IPlaylistService playlistService) =>
            _playlistService = playlistService;

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

            var elapsedMs = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine(
                $"\n{DateTime.Now.ToLocalTime()}: It took {elapsedMs} seconds to get the current user's playlists."
            );

            await _playlistService.UpdatePlaylistsAsync(command.AccessToken, playlists, ct);

            sw.Stop();
            Console.WriteLine(
                $"\n{DateTime.Now.ToLocalTime()}: It took {sw.Elapsed.TotalMilliseconds - elapsedMs} seconds to update the current user's playlists."
            );

            return playlists.Length;
        }
    }
}
