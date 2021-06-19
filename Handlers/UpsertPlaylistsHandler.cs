using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.Repositories;
using Playlister.Requests;

namespace Playlister.Handlers
{
    /// <summary>
    /// Add or Update the Playlists in request to the db.
    /// </summary>
    public class UpsertPlaylistsHandler : IRequestHandler<UpsertPlaylistsRequest, Unit>
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ILogger<UpsertPlaylistsHandler> _logger;

        public UpsertPlaylistsHandler(IPlaylistRepository playlistRepository, ILogger<UpsertPlaylistsHandler> logger)
        {
            _playlistRepository = playlistRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpsertPlaylistsRequest request, CancellationToken ct)
        {
            await _playlistRepository.Upsert(request.Playlists, ct);

            return new Unit();
        }
    }
}
