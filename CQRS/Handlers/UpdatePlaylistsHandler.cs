using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;
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

        public UpdatePlaylistsHandler(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        public async Task<Unit> Handle(UpdatePlaylistsCommand command, CancellationToken ct)
        {
            foreach (SimplifiedPlaylistObject playlist in command.Playlists)
            {
                await _playlistService.UpdatePlaylist(playlist.Id, 0, 50, ct);
            }

            return new Unit();
        }
    }
}
