using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Requests;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Utilities;

namespace Playlister.CQRS.Handlers
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

        public async Task<UserAccessInfo> Handle(AccessTokenRequest request, CancellationToken ct)
        {
            // TODO: validate that the `state` value matches the original value sent to user
            // TODO: Generate a client token to return so that the Spotify Access Token is never exposed outside the API
            AccessInfo info = await _api.AccessToken(
                new AccessTokenRequest.BodyParams
                {
                    Code = request.Code,
                    RedirectUri = _options.CallbackUrl.ToString(),
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret
                }, ct);

            return TokenUtility.CreateUserAccessToken(info, _cache, _logger);
        }
    }
}
