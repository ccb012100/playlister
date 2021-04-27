using System;
using System.Threading.Tasks;
using Playlister.HttpClients;

namespace Playlister.Services
{
    public class SpotifyAccountsService : ISpotifyAccountsService
    {
        private readonly ISpotifyAccountsApi _accountsApi;
        private readonly string _clientId;

        public SpotifyAccountsService(ISpotifyAccountsApi authApi, string clientId)
        {
            _accountsApi = authApi;
            _clientId = clientId;
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
