using System.Security.Authentication;
using Flurl;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Queries;
using Playlister.Models.SpotifyAccounts;
using Playlister.RefitClients;

namespace Playlister.Services;

public class AuthService : IAuthService
{
    private const string AuthScope = "user-read-private";
    private static string s_state = Guid.Empty.ToString();

    private readonly SpotifyOptions _options;

    private readonly ISpotifyAccountsApi _spotifyAccountsApi;

    public AuthService( ISpotifyAccountsApi api, IOptions<SpotifyOptions> options )
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
        s_state = Guid.NewGuid().ToString();

        return _options.AccountsApiBaseAddress
            .AppendPathSegment( "authorize" )
            .AppendQueryParam( "response_type", "code" )
            .AppendQueryParam( "client_id", _options.ClientId )
            .AppendQueryParam( "redirect_uri", _options.CallbackUrl )
            .AppendQueryParam( "scope", AuthScope )
            .AppendQueryParam( "state", s_state )
            .ToUri();
    }

    public async Task<Guid> GetAccessToken( AuthorizationResult auth, CancellationToken ct = default )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( auth.Code );
        ArgumentException.ThrowIfNullOrWhiteSpace( auth.State );

        if (auth.State != s_state)
        {
            throw new InvalidCredentialException( $"Invalid 'state' value: \"{s_state}\"! Expected \"{s_state}\"" );
        }

        SpotifyAccessToken token = await _spotifyAccountsApi.RequestAccessTokenAsync(
            new GetAccessTokenQuery.BodyParams
            {
                Code = auth.Code,
                RedirectUri = _options.CallbackUrl.ToString(),
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            },
            ct
        );

        return TokenService.SetToken( token.ToUserAccessToken() );
    }
}
