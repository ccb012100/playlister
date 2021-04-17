using System;

namespace Playlister.HttpClients
{
    public class SpotifyApi : ISpotifyApi
    {
        private readonly Uri _apiBaseUrl;

        public SpotifyApi(Uri apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl;
        }
    }
}
