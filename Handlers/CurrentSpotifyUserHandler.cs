using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.HttpClients;
using Playlister.Models.Spotify;
using Playlister.Requests;

// ReSharper disable UnusedType.Global

namespace Playlister.Handlers
{
    public class CurrentSpotifyUserHandler : IRequestHandler<CurrentSpotifyUserRequest, PrivateUserObject>
    {
        private readonly ISpotifyApi _spotifyApi;

        public CurrentSpotifyUserHandler(ISpotifyApi spotifyApi)
        {
            _spotifyApi = spotifyApi;
        }

        public async Task<PrivateUserObject> Handle(CurrentSpotifyUserRequest request, CancellationToken cancellationToken)
        {
            return await _spotifyApi.GetCurrentUser(request.AccessToken, cancellationToken);
        }
    }
}
