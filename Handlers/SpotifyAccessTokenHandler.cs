using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Requests;
using Playlister.Utilities;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyAccessTokenHandler : IRequestHandler<AccessTokenRequest, UserAccessInfo>
    {
        private readonly ISpotifyAccountsApi _api;
        private readonly ILogger<SpotifyAccessTokenHandler> _logger;
        private readonly SpotifyOptions _options;
        private readonly IMemoryCache _cache;

        public SpotifyAccessTokenHandler(ISpotifyAccountsApi api, IOptions<SpotifyOptions> options,
            ILogger<SpotifyAccessTokenHandler> logger, IMemoryCache cache)
        {
            _api = api;
            _logger = logger;
            _cache = cache;
            _options = options.Value;
        }

        public async Task<UserAccessInfo> Handle(AccessTokenRequest request, CancellationToken cancellationToken)
        {
            // TODO: validate that the `state` value matches the original value sent to user
            // TODO: Generate a client token to return so that the Spotify Access Token is never exposed outside the API
            AccessInfo info = await _api.AccessToken(
                new AccessTokenRequestParams
                {
                    Code = request.Code,
                    RedirectUri = _options.CallbackUrl.ToString(),
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret
                }, cancellationToken);

            return TokenUtility.CreateUserAccessToken(info, _cache, _logger);
        }
    }
}
