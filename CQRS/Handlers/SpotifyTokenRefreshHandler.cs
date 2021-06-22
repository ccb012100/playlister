using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Requests;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.RefitClients;
using Playlister.Repositories;

namespace Playlister.CQRS.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyTokenRefreshHandler : IRequestHandler<TokenRefreshRequest, UserAccessToken>
    {
        private readonly ISpotifyAccountsApi _api;
        private readonly ILogger<SpotifyTokenRefreshHandler> _logger;
        private readonly SpotifyOptions _options;
        private readonly IAccessTokenRepository _tokenRepository;

        public SpotifyTokenRefreshHandler(ISpotifyAccountsApi api, ILogger<SpotifyTokenRefreshHandler> logger,
            IOptions<SpotifyOptions> options, IAccessTokenRepository tokenRepository)
        {
            _api = api;
            _logger = logger;
            _options = options.Value;
            _tokenRepository = tokenRepository;
        }

        public async Task<UserAccessToken> Handle(TokenRefreshRequest request, CancellationToken ct)
        {
            string authParam =
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

            SpotifyAccessToken token = await _api.RefreshToken(authParam,
                new TokenRefreshRequest.BodyParams(request.RefreshToken), ct);

            return await _tokenRepository.AddToken(token);
        }
    }
}
