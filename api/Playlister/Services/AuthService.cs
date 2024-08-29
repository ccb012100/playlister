using Flurl;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyAccounts;
using Playlister.RefitClients;

namespace Playlister.Services
{
    public class AuthService : IAuthService
    {
        private const string AuthScope = "user-read-private";

        private readonly ISpotifyAccountsApi _spotifyAccountsApi;
        private readonly SpotifyOptions _options;

        public AuthService(ISpotifyAccountsApi api, IOptions<SpotifyOptions> options)
        {
            _options = options.Value;
            _spotifyAccountsApi = api;
        }

        public Uri GetSpotifyAuthUrl()
        {
            /*
             * https://accounts.spotify.com/authorize?
             * client_id=5fe01282e44241328a84e7c5cc169165
             * &response_type=code
             * &redirect_uri=https%3A%2F%2Fexample.com%2Fcallback
             * &scope=user-read-private%20user-read-email
             * &state=34fFs29kd09
             */
            // TODO: cache `state` so that it can be validated on the access token command

            return _options.AccountsApiBaseAddress.AppendPathSegment("authorize")
                .AppendQueryParam("response_type", "code")
                .AppendQueryParam("client_id", _options.ClientId)
                .AppendQueryParam("redirect_uri", _options.CallbackUrl)
                .AppendQueryParam("scope", AuthScope)
                .AppendQueryParam("state", Guid.NewGuid())
                .ToUri();
        }

        public async Task<Guid> GetAccessToken(string code, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            // TODO: validate that the `state` value matches the original value sent to user
            // TODO: Generate a client token to return so that the Spotify Access Token is never exposed outside the API
            SpotifyAccessToken token = await _spotifyAccountsApi.RequestAccessTokenAsync(
                new GetAccessTokenCommand.BodyParams
                {
                    Code = code, RedirectUri = _options.CallbackUrl.ToString(), ClientId = _options.ClientId, ClientSecret = _options.ClientSecret
                }, ct);

            return TokenService.AddToken(token.ToUserAccessToken());
        }
    }
}
