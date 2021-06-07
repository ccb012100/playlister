using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Playlister.HttpClients;
using Playlister.Requests;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyAccountAuthorizationHandler : IRequestHandler<AuthorizationRequest, string>
    {
        private readonly ISpotifyAccountsApi _accountsApi;
        private readonly SpotifyOptions _options;

        public SpotifyAccountAuthorizationHandler(ISpotifyAccountsApi accountsApi, IOptions<SpotifyOptions> options)
        {
            _accountsApi = accountsApi;
            _options = options.Value;
        }

        public async Task<string> Handle(AuthorizationRequest request, CancellationToken cancellationToken)
        {
            return await _accountsApi.Authorize(
                new AuthQueryParams
                {
                    ClientId = _options.ClientId,
                    RedirectUri = _options.CallbackUrl,
                    State = Guid.NewGuid().ToString(),
                    Scope = null,
                    ShowDialog = false
                },
                cancellationToken
            );
        }
    }
}
