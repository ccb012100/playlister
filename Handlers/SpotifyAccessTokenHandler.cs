using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.HttpClients;
using Playlister.Models;
using Playlister.Requests;
using Refit;

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
            IApiResponse<SpotifyAccessToken> apiResponse = await _accountsApi.AccessToken(
                new AccessTokenRequestParams
                {
                    Code = request.Code,
                    RedirectUri = _options.CallbackUrl.ToString(),
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret
                }, cancellationToken);

            if (!apiResponse.IsSuccessStatusCode)
            {
                // All failures result in Spotify returning a 400 Bad Request
                _logger.LogError(
                    $"Call to get Access Token failed: `{apiResponse.Error?.ReasonPhrase} {apiResponse.Error?.Content}`");

                throw apiResponse.Error!;
            }

            SpotifyAccessToken spotifyToken = apiResponse.Content!;

            var userToken = new UserAccessToken
            {
                AccessToken = spotifyToken.AccessToken,
                Expiration = DateTime.Now.AddSeconds(spotifyToken.ExpiresIn),
                RefreshToken = spotifyToken.RefreshToken,
                Scopes = spotifyToken.Scope.Split(' ')
            };

            // use AccessToken as cache key
            _cache.Set(spotifyToken.AccessToken, userToken, TimeSpan.FromSeconds(spotifyToken.ExpiresIn));

            return userToken;
        }
    }
}
