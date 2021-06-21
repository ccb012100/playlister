using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.CQRS.Requests;
using Playlister.Repositories;

namespace Playlister.CQRS.Handlers
{
    /// <summary>
    /// Add or Update the Playlists in request to the db.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UpsertPlaylistsHandler : IRequestHandler<UpsertPlaylistsRequest, Unit>
    {
        private readonly IPlaylistRepository _playlistRepository;

        public UpsertPlaylistsHandler(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        public async Task<Unit> Handle(UpsertPlaylistsRequest request, CancellationToken ct)
        {
            await _playlistRepository.Upsert(request.Playlists, ct);
            return new Unit();
        }
    }
}
