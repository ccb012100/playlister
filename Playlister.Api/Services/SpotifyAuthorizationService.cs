using Playlister.HttpClients;

namespace Playlister.Services
{
    public class SpotifyAuthorizationService : ISpotifyAuthorizationService
    {
        private readonly ISpotifyAuthorizationApi _authorizationApi;

        public SpotifyAuthorizationService(ISpotifyAuthorizationApi authApi)
        {
            _authorizationApi = authApi;
        }
    }
}
