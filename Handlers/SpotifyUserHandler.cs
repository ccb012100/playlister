using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Models.Spotify;
using Playlister.Requests;

// ReSharper disable UnusedType.Global

namespace Playlister.Handlers
{
    public class SpotifyUserHandler : IRequestHandler<SpotifyUserRequest, UserProfile>
    {
        private readonly ISpotifyApi _spotifyApi;

        public SpotifyUserHandler(ISpotifyApi spotifyApi)
        {
            _spotifyApi = spotifyApi;
        }

        public async Task<UserProfile> Handle(SpotifyUserRequest request, CancellationToken cancellationToken)
        {
            return await _spotifyApi.GetUser(request.AccessToken, cancellationToken);
        }
    }
}
