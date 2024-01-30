using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Sync the Playlist in the command to the db, regardless of whether it's up-to-date or not.
    /// </summary>
    public class SyncPlaylistsHandler : IRequestHandler<SyncPlaylistCommand, Unit>
    {
        private readonly IPlaylistService _playlistService;

        public SyncPlaylistsHandler(IPlaylistService playlistService) => _playlistService = playlistService;

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns>Number of playlists handled</returns>
        public async Task<Unit> Handle(SyncPlaylistCommand command, CancellationToken ct)
        {
            await Task.Run(() => _playlistService.SyncPlaylistAsync(command.AccessToken, command.PlaylistId, ct), ct);

            return new Unit();
        }
    }
}
