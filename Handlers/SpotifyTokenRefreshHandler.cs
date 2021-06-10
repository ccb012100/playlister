using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Requests;
using Playlister.Utilities;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyTokenRefreshHandler : IRequestHandler<TokenRefreshRequest, UserAccessToken>
    {
        private readonly ISpotifyAccountsApi _accountsApi;
        private readonly ILogger<SpotifyTokenRefreshHandler> _logger;
        private readonly SpotifyOptions _options;
        private readonly IMemoryCache _cache;

        public SpotifyTokenRefreshHandler(ISpotifyAccountsApi accountsApi, ILogger<SpotifyTokenRefreshHandler> logger,
            IOptions<SpotifyOptions> options, IMemoryCache cache)
        {
            _accountsApi = accountsApi;
            _logger = logger;
            _options = options.Value;
            _cache = cache;
        }

        public async Task<UserAccessToken> Handle(TokenRefreshRequest request, CancellationToken cancellationToken)
        {
            string authParam =
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

            SpotifyAccessToken spotifyToken = await _accountsApi.RefreshToken(authParam,
                new TokenRefreshRequestParams(request.RefreshToken), cancellationToken);

            return TokenUtility.CreateUserAccessToken(spotifyToken, _cache, _logger);
        }
    }
}
