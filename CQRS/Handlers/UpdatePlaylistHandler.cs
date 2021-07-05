using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Update the list of Tracks for the specified playlist.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpdatePlaylistHandler : IRequestHandler<UpdatePlaylistCommand, Unit>
    {
        private readonly IPlaylistService _playlistService;

        public UpdatePlaylistHandler(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        public async Task<Unit> Handle(UpdatePlaylistCommand command, CancellationToken ct)
        {
            await _playlistService.UpdatePlaylist(command, ct);

            return new Unit();
        }
    }
}
