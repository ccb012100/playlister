using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.HttpClients;
using Playlister.Models.SpotifyApi;
using Playlister.Requests;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class CurrentSpotifyUserPlaylistsHandler : IRequestHandler<CurrentUserPlaylistsRequest,
        PagingObject<SimplifiedPlaylistObject>>
    {
        private readonly ISpotifyApi _api;

        public CurrentSpotifyUserPlaylistsHandler(ISpotifyApi api)
        {
            _api = api;
        }

        public async Task<PagingObject<SimplifiedPlaylistObject>> Handle(CurrentUserPlaylistsRequest request,
            CancellationToken ct)
        {
            return await _api.GetCurrentUserPlaylists(request, ct);
        }
    }
}
