using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.HttpClients;
using Playlister.Requests;
using Refit;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyAccessTokenHandler : IRequestHandler<AccessTokenRequest, Unit>
    {
        private readonly ISpotifyAccountsApi _accountsApi;
        private readonly ILogger<SpotifyAccessTokenHandler> _logger;
        private readonly SpotifyOptions _options;

        public SpotifyAccessTokenHandler(ISpotifyAccountsApi accountsApi, IOptions<SpotifyOptions> options,
            ILogger<SpotifyAccessTokenHandler> logger)
        {
            _accountsApi = accountsApi;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<Unit> Handle(AccessTokenRequest request, CancellationToken cancellationToken)
        {
            IApiResponse<SpotifyAccessToken> apiResponse = await _accountsApi.AccessToken(
                new AccessTokenRequestParams
                {
                    Code = request.Code,
                    RedirectUri = _options.CallbackUrl.ToString(),
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret
                }, cancellationToken);

            if (apiResponse.IsSuccessStatusCode)
            {
                return Unit.Value;
            }

            _logger.LogError($"Call to get Access Token failed: `{apiResponse.Error?.Content}`");

            throw apiResponse.Error!;
        }
    }
}
