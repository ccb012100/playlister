using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.HttpClients;
using Playlister.Models.SpotifyApi;
using Playlister.Requests;

namespace Playlister.Handlers
{
    public class
        // ReSharper disable once UnusedType.Global
        CurrentSpotifyUserPlaylistsHandler : IRequestHandler<CurrentUserPlaylistsRequest,
            PagingObject<PlaylistObject>>
    {
        private readonly ISpotifyApi _api;

        public CurrentSpotifyUserPlaylistsHandler(ISpotifyApi api)
        {
            _api = api;
        }

        public async Task<PagingObject<PlaylistObject>> Handle(CurrentUserPlaylistsRequest request,
            CancellationToken cancellationToken)
        {
            return await _api.GetCurrentUserPlaylists(request, cancellationToken);
        }
    }
}
