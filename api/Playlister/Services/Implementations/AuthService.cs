using System.Security.Authentication;
using System.Text;

using Flurl;

using Microsoft.Extensions.Options;

using Playlister.Configuration;
using Playlister.CQRS.Queries;
using Playlister.Models.SpotifyAccounts;
using Playlister.Models.SpotifyApi;
using Playlister.RefitClients;

namespace Playlister.Services.Implementations;

public class AuthService(
    ISpotifyAccountsApi accountsApi ,
    ISpotifyApi spotifyApi ,
    IOptions<SpotifyOptions> options ,
    ILogger<AuthService> logger
)
    : IAuthService {
    private const string AuthScope = "user-read-private";
    private static string s_state = Guid.Empty.ToString( );

    private readonly ILogger<AuthService> _logger = logger;
    private readonly SpotifyOptions _options = options.Value;
    private readonly ISpotifyAccountsApi _spotifyAccountsAccountsApi = accountsApi;
    private readonly ISpotifyApi _spotifyApi = spotifyApi;

    /// <summary>
    ///     See <a href="https://developer.spotify.com/documentation/web-api/tutorials/code-pkce-flow">Spotify Developer Documentation</a>
    /// </summary>
    public Uri GetSpotifyAuthUrl( ) {
        s_state = Guid.NewGuid( ).ToString( );

        return _options.AccountsApiBaseAddress
            .AppendPathSegment( "authorize" )
            .AppendQueryParam( "response_type" , "code" )
            .AppendQueryParam( "client_id" , _options.ClientId )
            .AppendQueryParam( "redirect_uri" , _options.CallbackUrl )
            .AppendQueryParam( "scope" , AuthScope )
            .AppendQueryParam( "state" , s_state )
            .ToUri( );
    }

    public async Task<Guid> GetAccessToken( AuthorizationResult auth , CancellationToken ct = default ) {
        ArgumentException.ThrowIfNullOrWhiteSpace( auth.Code );
        ArgumentException.ThrowIfNullOrWhiteSpace( auth.State );

        if ( s_state == Guid.Empty.ToString( ) ) {
            throw new InvalidOperationException( "No Auth URL has been generated yet!" );
        }

        if ( auth.State != s_state ) {
            throw new InvalidCredentialException( $"Invalid 'state' value: \"{auth.State}\"! Expected \"{s_state}\"" );
        }

        SpotifyAccessToken token = await _spotifyAccountsAccountsApi.RequestAccessTokenAsync(
            AuthHeaderValue( ) ,
            new AccessTokenRequestParams {
                Code = auth.Code ,
                RedirectUri = _options.CallbackUrl.ToString( )
            } ,
            ct
        );

        _logger.LogDebug( "Successfully requested access token from Spotify API" );

        PrivateUserObject user = await _spotifyApi.GetCurrentUserAsync( token.AccessToken , ct );

        // Make sure the current user is the user configured for this application
        if ( user.Uri.ToString( ) == _options.SpotifyUri ) {
            return TokenService.SetToken( token.ToUserAccessToken( ) );
        }

        _logger.LogError( "Invalid user: {User}" , user );

        throw new InvalidOperationException( $"Invalid user. Expected: '{_options.SpotifyUri}'. Got: '{user.Uri}'" );
    }

    public async Task<Guid> RefreshSpotifyToken( RefreshTokenQuery query , CancellationToken ct = default ) {
        SpotifyAccessToken token = await _spotifyAccountsAccountsApi.RefreshTokenAsync(
            AuthHeaderValue( ) ,
            new TokenRefreshParams( query.RefreshToken ) ,
            ct
        );

        _logger.LogDebug( "Successfully refreshed access token from Spotify API" );

        return TokenService.RefreshToken( token.ToUserAccessToken( ) );
    }

    private string AuthHeaderValue( ) {
        return Convert.ToBase64String( Encoding.UTF8.GetBytes( $"{_options.ClientId}:{_options.ClientSecret}" ) );
    }
}
