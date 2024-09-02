using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.CQRS.Handlers;
using Playlister.ViewModels;

namespace Playlister.Controllers;

public class HomeController : Controller
{
    public const string Name = "Home";
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly SpotifyAuthUrlHandler _spotifyAuthUrlHandler;

    public HomeController( IHostApplicationLifetime appLifetime, SpotifyAuthUrlHandler spotifyAuthUrlHandler )
    {
        _appLifetime = appLifetime;
        _spotifyAuthUrlHandler = spotifyAuthUrlHandler;
    }

    /// Navigate to Spotify login
    public async Task<IActionResult> Index()
    {
        // TODO: check for auth cookie and if it's already there, just redirect to Main()
        return Redirect( (await _spotifyAuthUrlHandler.Handle( new GetAuthUrlCommand() )).ToString() );
    }

    public IActionResult Main()
    {
        return View( new HomeViewModel() );
    }

    [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
    public IActionResult Error( int? statusCode )
    {
        return statusCode switch
        {
            404 => View( "PageNotFound", new PageNotFoundViewModel() ),
            _ => View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    ResponseStatusCode = statusCode
                }
            )
        };
    }

    /// <summary>
    ///     404 Page
    /// </summary>
    /// <returns></returns>
    [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
    public IActionResult PageNotFound()
    {
        return View( new PageNotFoundViewModel() );
    }

    /// <summary>
    ///     Stop the application
    /// </summary>
    /// <returns></returns>
    [ValidateTokenCookie]
    [HttpPost( "stop-application" )]
    public ActionResult StopApplication()
    {
        Task.Factory.StartNew(
            () =>
            {
                Thread.Sleep( 3_000 );
                _appLifetime.StopApplication();
            }
        );

        return NoContent();
    }
}
