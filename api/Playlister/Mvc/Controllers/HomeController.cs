using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Handlers;
using Playlister.Mvc.ViewModels;
using Playlister.Services;

namespace Playlister.Mvc.Controllers;

public class HomeController : Controller
{
    public const string Name = "Home";
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly SpotifyAuthorizationHandler _spotifyAuthorizationHandler;

    public HomeController( IHostApplicationLifetime appLifetime, SpotifyAuthorizationHandler spotifyAuthorizationHandler )
    {
        _appLifetime = appLifetime;
        _spotifyAuthorizationHandler = spotifyAuthorizationHandler;
    }

    /// <summary>
    ///     Navigate to Spotify login
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType<ViewResult>( StatusCodes.Status200OK )]
    public async Task<IActionResult> Index()
    {
        return Redirect(
            TokenService.TryValidateCookie(
                HttpContext.Request.Cookies[TokenService.UserTokenCookieName],
                out string? _
            )
                ? "/Home/Main/"
                : (await _spotifyAuthorizationHandler.GetAuthorizationUrl()).ToString()
        );
    }

    [ProducesResponseType<ViewResult>( StatusCodes.Status200OK )]
    public IActionResult Main()
    {
        return View( new HomeViewModel() );
    }

    [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
    [ProducesResponseType<ViewResult>( StatusCodes.Status200OK )]
    public IActionResult Error( int? statusCode )
    {
        switch (statusCode)
        {
            case 404:
                {
                    IStatusCodeReExecuteFeature? reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

                    return View(
                        "PageNotFound",
                        new PageNotFoundViewModel(
                            reExecute is not null
                                ? $"{reExecute.OriginalPathBase}{reExecute.OriginalPath}{reExecute.OriginalQueryString}"
                                : $"{HttpContext.Request.PathBase}{HttpContext.Request.Path}{HttpContext.Request.QueryString}"
                        )
                    );
                }
            default:
                IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

                string exceptionMessage = exceptionHandlerPathFeature?.Error.Message ?? "<none>";

                return View(
                    new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                        ResponseStatusCode = statusCode,
                        ExceptionMessage = exceptionMessage
                    }
                );
        }
    }

    /// <summary>
    ///     Stop the application
    /// </summary>
    /// <returns></returns>
    [ValidateTokenCookie]
    [HttpPost( "stop-application" )]
    [ProducesResponseType( StatusCodes.Status204NoContent )]
    public ActionResult StopApplication()
    {
        Task.Factory.StartNew(
            () =>
            {
                Thread.Sleep( 1_000 ); // gives the application time to return a response
                _appLifetime.StopApplication();
            }
        );

        return NoContent();
    }
}
