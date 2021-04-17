namespace Playlister.HttpClients
{
    public class SpotifyAuthorizationApi : ISpotifyAuthorizationApi
    {
        private readonly string _clientId;

        public SpotifyAuthorizationApi(string clientId)
        {
            _clientId = clientId;
        }
    }
}
