using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
    public class SpotifyAccessTokenHandler : IRequestHandler<RequestAccessTokenCommand, UserAccessToken>
    {
        private readonly ISpotifyAccountsApi _api;
        private readonly SpotifyOptions _options;
        private readonly IAccessTokenRepository _tokenRepository;

        public SpotifyAccessTokenHandler(ISpotifyAccountsApi api, IOptions<SpotifyOptions> options,
            IAccessTokenRepository tokenRepository)
        {
            _api = api;
            _tokenRepository = tokenRepository;
            _options = options.Value;
        }

        public async Task<UserAccessToken> Handle(RequestAccessTokenCommand command, CancellationToken ct)
        {
            // TODO: validate that the `state` value matches the original value sent to user
            // TODO: Generate a client token to return so that the Spotify Access Token is never exposed outside the API
            SpotifyAccessToken token = await _api.AccessToken(
                new RequestAccessTokenCommand.BodyParams
                {
                    Code = command.Code,
                    RedirectUri = _options.CallbackUrl.ToString(),
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret
                }, ct);

            return await _tokenRepository.AddToken(token);
        }
    }
}
