using System.Collections.Immutable;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    ///     Add or Update the current user's playlists to the db.
    /// </summary>
    public class UpdateCurrentUserPlaylistsHandler : IRequestHandler<UpdateCurrentUserPlaylistsCommand, (int total, int updated)>
    {
        private readonly IPlaylistService _playlistService;

        public UpdateCurrentUserPlaylistsHandler(IPlaylistService playlistService) => _playlistService = playlistService;

        /// <summary>
        ///     Update Current user's playlists
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns>Number of playlists Updated.</returns>
        public async Task<(int total, int updated)> Handle(UpdateCurrentUserPlaylistsCommand command, CancellationToken ct)
        {
            ImmutableArray<Playlist> playlists = await _playlistService.GetCurrentUserPlaylistsAsync(command.AccessToken, ct);

            int updated = await _playlistService.UpdatePlaylistsAsync(command.AccessToken, playlists, ct);
            await _playlistService.DeleteOrphanedPlaylistTracksAsync(ct);

            return (total: playlists.Length, updated);
        }
    }
}
