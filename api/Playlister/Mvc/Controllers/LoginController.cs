using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Playlister.CQRS.Handlers;
using Playlister.CQRS.Queries;
using Playlister.Services.Implementations;

namespace Playlister.Mvc.Controllers;

public class LoginController(
    ILogger<LoginController> logger ,
    SpotifyAuthorizationHandler spotifyAuthorizationHandler ,
    IHttpContextAccessor httpContextAccessor
) : Controller {
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<LoginController> _logger = logger;
    private readonly SpotifyAuthorizationHandler _spotifyAuthorizationHandler = spotifyAuthorizationHandler;

    /// <summary>
    ///     URL that Spotify redirects to after authenticating with Spotify's accounts URL.
    /// </summary>
    /// <param name="code">The reason authorization failed, for example: <c>"access_denied"</c></param>
    /// <param name="state">The value of the <c>state</c> parameter supplied in the request; used to prevent CSRF attacks</param>
    /// <param name="error">Error message populated if authentication failed</param>
    /// <param name="returnUrl">Local URL to redirect to after successful authentication</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"><paramref name="error" /> is not <c>null</c></exception>
    [ProducesResponseType<LocalRedirectResult>( StatusCodes.Status307TemporaryRedirect )]
    [ProducesResponseType<RedirectToActionResult>( StatusCodes.Status302Found )]
    public async Task<IActionResult> Index(
        [FromQuery] string code ,
        [FromQuery] string state ,
        [FromQuery] string? error ,
        [FromQuery] string? returnUrl ,
        CancellationToken ct = default
    ) {
        // Spotify sets the "error" query param if authentication failed
        if ( error is not null ) {
            _logger.LogError( "Spotify auth returned an error: {AuthError}" , error.ReplaceLineEndings( string.Empty ) );

            throw new InvalidOperationException( $"Error authenticating with Spotify: {error}" );
        }

        Guid viewToken = await _spotifyAuthorizationHandler.GetSpotifyAccessToken(
            new GetAccessTokenQuery {
                Code = code ,
                State = state
            } ,
            ct
        );

        ClaimsIdentity claimsIdentity = new( Array.Empty<Claim>( ) , CookieAuthenticationDefaults.AuthenticationScheme );

        AuthenticationProperties authProperties = new( ) {
            AllowRefresh = true ,
            IsPersistent = true ,
            IssuedUtc = DateTimeOffset.Now
            // TODO: set RedirectUri
        };

        await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme , new ClaimsPrincipal( claimsIdentity ) , authProperties );

        _logger.LogInformation( "Successful login" );

        Response.Headers.Authorization = new StringValues( $"Bearer {viewToken}" );

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            TokenService.UserTokenCookieName ,
            viewToken.ToString( ) ,
            new CookieOptions {
                Domain = _httpContextAccessor.HttpContext.Request.Host.Host ,
                Expires = TokenService.GetTokenExpirationUtc( viewToken ) ,
                HttpOnly = true ,
                IsEssential = true ,
                Path = "/" , // this needs to be set so that the Cookie is not set only for the redirect URL
                Secure = true
            }
        );

        return returnUrl is not null
            ? LocalRedirectPreserveMethod( returnUrl )
            : RedirectToAction( nameof( HomeController.Main ) , HomeController.Name );
    }
}
