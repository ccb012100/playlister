using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.RefitClients;
using Playlister.Repositories;

namespace Playlister.CQRS.Handlers
{
    // ReSharper disable once UnusedType.Global
    public class SpotifyTokenRefreshHandler : IRequestHandler<RefreshTokenCommand, UserAccessToken>
    {
        private readonly ISpotifyAccountsApi _api;
        private readonly SpotifyOptions _options;
        private readonly IAccessTokenRepository _tokenRepository;

        public SpotifyTokenRefreshHandler(ISpotifyAccountsApi api, IOptions<SpotifyOptions> options,
            IAccessTokenRepository tokenRepository)
        {
            _api = api;
            _options = options.Value;
            _tokenRepository = tokenRepository;
        }

        public async Task<UserAccessToken> Handle(RefreshTokenCommand command, CancellationToken ct)
        {
            string authParam =
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

            SpotifyAccessToken token = await _api.RefreshTokenAsync(authParam,
                new RefreshTokenCommand.BodyParams(command.RefreshToken), ct);

            return await _tokenRepository.AddTokenAsync(token);
        }
    }
}
