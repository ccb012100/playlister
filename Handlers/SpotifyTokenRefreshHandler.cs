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
using Playlister.Models.SpotifyAccounts;
using Playlister.Requests;
using Playlister.Utilities;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyTokenRefreshHandler : IRequestHandler<TokenRefreshRequest, UserAccessInfo>
    {
        private readonly ISpotifyAccountsApi _api;
        private readonly ILogger<SpotifyTokenRefreshHandler> _logger;
        private readonly SpotifyOptions _options;
        private readonly IMemoryCache _cache;

        public SpotifyTokenRefreshHandler(ISpotifyAccountsApi api, ILogger<SpotifyTokenRefreshHandler> logger,
            IOptions<SpotifyOptions> options, IMemoryCache cache)
        {
            _api = api;
            _logger = logger;
            _options = options.Value;
            _cache = cache;
        }

        public async Task<UserAccessInfo> Handle(TokenRefreshRequest request, CancellationToken cancellationToken)
        {
            string authParam =
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

            AccessInfo info = await _api.RefreshToken(authParam,
                new TokenRefreshRequestParams(request.RefreshToken), cancellationToken);

            return TokenUtility.CreateUserAccessToken(info, _cache, _logger);
        }
    }
}
