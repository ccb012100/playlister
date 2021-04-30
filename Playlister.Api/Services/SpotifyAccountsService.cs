using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Playlister.Api.HttpClients;

namespace Playlister.Api.Services
{
    public class SpotifyAccountsService : ISpotifyAccountsService
    {
        private readonly ISpotifyAccountsApi _accountsApi;
        private readonly SpotifyOptions _options;

        public SpotifyAccountsService(ISpotifyAccountsApi accountsApi, IOptions<SpotifyOptions> options)
        {
            _accountsApi = accountsApi;
            _options = options.Value;
        }

        public async Task<object> Authorize()
        {
            return await _accountsApi.Authorize(
                _options.ClientId,
                _options.CallbackUrl,
                Guid.NewGuid().ToString(),
                null,
                false
            );
        }
    }
}
