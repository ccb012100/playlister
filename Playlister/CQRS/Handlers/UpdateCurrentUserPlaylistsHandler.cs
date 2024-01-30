using System.Collections.Immutable;
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
        /// Update Current user's playlists
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns>Number of playlists Updated.</returns>
        public async Task<int> Handle(
            UpdateCurrentUserPlaylistsCommand command,
            CancellationToken ct
        )
        {
            ImmutableArray<Playlist> playlists =
                await _playlistService.GetCurrentUserPlaylistsAsync(command.AccessToken, ct)!;

            await _playlistService.UpdatePlaylistsAsync(command.AccessToken, playlists, ct);
            await _playlistService.DeleteOrphanedPlaylistTracksAsync(ct);

            return playlists.Length;
        }
    }
}
