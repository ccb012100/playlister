using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Playlister.HttpClients;

namespace Playlister.Services
{
    public class SpotifyAccountsService : ISpotifyAccountsService
    {
        private readonly ISpotifyAccountsApi _accountsApi;
        private readonly string _clientId;

        public SpotifyAccountsService(ISpotifyAccountsApi authApi, IOptions<SpotifyOptions> spotifyOptions)
        {
            _accountsApi = authApi;
            _clientId = spotifyOptions.Value.ClientId;
        }

        public async Task<object> Authorize()
        {
            return await _accountsApi.Authorize(
                _clientId,
                new Uri("https://localhost:8001"),
                Guid.NewGuid().ToString(),
                string.Empty,
                false
            );
        }
    }
}
