using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the Playlists in command to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistsHandler : IRequestHandler<UpdatePlaylistsCommand, Unit>
    {
        private readonly IPlaylistService _playlistService;

        public UpdatePlaylistsHandler(IPlaylistService playlistService) => _playlistService = playlistService;

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns>Number of playlists handled</returns>
        // ReSharper disable once UseDeconstructionOnParameter
        public async Task<Unit> Handle(UpdatePlaylistsCommand command, CancellationToken ct)
        {
            await _playlistService.UpdatePlaylistsAsync(command.AccessToken,
                command.Playlists.Select(p => p.ToPlaylist()), ct);

            return new Unit();
        }
    }
}
