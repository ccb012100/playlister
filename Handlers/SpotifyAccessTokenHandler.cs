using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Requests;

namespace Playlister.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyAccessTokenHandler : IRequestHandler<AccessTokenRequest, UserAccessToken>
    {
        private readonly ISpotifyAccountsApi _accountsApi;
        private readonly ILogger<SpotifyAccessTokenHandler> _logger;
        private readonly SpotifyOptions _options;
        private readonly IMemoryCache _cache;

        public SpotifyAccessTokenHandler(ISpotifyAccountsApi accountsApi, IOptions<SpotifyOptions> options,
            ILogger<SpotifyAccessTokenHandler> logger, IMemoryCache cache)
        {
            _accountsApi = accountsApi;
            _logger = logger;
            _cache = cache;
            _options = options.Value;
        }

        public async Task<UserAccessToken> Handle(AccessTokenRequest request, CancellationToken cancellationToken)
        {
            SpotifyAccessToken spotifyToken = await _accountsApi.AccessToken(
                new AccessTokenRequestParams
                {
                    Code = request.Code,
                    RedirectUri = _options.CallbackUrl.ToString(),
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret
                }, cancellationToken);

            var userToken = new UserAccessToken
            {
                AccessToken = spotifyToken.AccessToken,
                Expiration = DateTime.Now.AddSeconds(spotifyToken.ExpiresIn),
                RefreshToken = spotifyToken.RefreshToken,
                Scopes = spotifyToken.Scope.Split(' ')
            };

            // use AccessToken as cache key
            _cache.Set(spotifyToken.AccessToken, userToken, TimeSpan.FromSeconds(spotifyToken.ExpiresIn));
            _logger.LogDebug($"Added access token key={spotifyToken.AccessToken} to cache.");

            return userToken;
        }
    }
}
