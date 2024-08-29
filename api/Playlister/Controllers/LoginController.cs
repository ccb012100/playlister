using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Playlister.Services;

namespace Playlister.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginController(ILogger<LoginController> logger, IAuthService authService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// URL that Spotify redirects to after authenticating with Spotify's accounts URL.
    /// </summary>
    /// <param name="code">The reason authorization failed, for example: <c>"access_denied"</c></param>
    /// <param name="state">The value of the <c>state</c> parameter supplied in the request; used to prevent CSRF attacks</param>
    /// <param name="error">Error message populated if authentication failed</param>
    /// <param name="returnUrl">Local URL to redirect to after successful authentication</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"><paramref name="error"/> is not <c>null</c></exception>
    public async Task<IActionResult> Index([FromQuery] string code, [FromQuery] string state, [FromQuery] string? error,
        [FromQuery] string? returnUrl)
    {
        // Spotify sets the "error" query param if authentication failed
        if (error is not null)
        {
            _logger.LogError("Spotify auth returned an error: {AuthError}", error);

            throw new InvalidOperationException($"Error authenticating with Spotify: {error}");
        }

        Guid viewToken = await _authService.GetAccessToken(code);

        ClaimsIdentity claimsIdentity = new(Array.Empty<Claim>(), CookieAuthenticationDefaults.AuthenticationScheme);

        AuthenticationProperties authProperties = new()
        {
            AllowRefresh = true, IsPersistent = true, IssuedUtc = DateTimeOffset.Now, // TODO: set RedirectUri
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        Response.Headers.Authorization = new StringValues($"Bearer {viewToken}");

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(TokenService.UserTokenCookieName, viewToken.ToString());

        return returnUrl is not null
            ? LocalRedirectPreserveMethod(returnUrl)
            : RedirectToAction(nameof(SyncController.Index), "Sync");
    }
}
