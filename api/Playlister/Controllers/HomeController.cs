using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.CQRS.Commands;
using Playlister.ViewModels;

namespace Playlister.Controllers;

public class HomeController : Controller
{
    public const string Name = "Home";

    private readonly IMediator _mediator;
    private readonly IHostApplicationLifetime _appLifetime;

    public HomeController(IMediator mediator, IHostApplicationLifetime appLifetime)
    {
        _mediator = mediator;
        _appLifetime = appLifetime;
    }

    /// Navigate to Spotify login
    public async Task<IActionResult> Index()
    {
        // TODO: check for auth cookie and if it's already there, just redirect to Sync
        return Redirect((await _mediator.Send(new GetAuthUrlCommand())).ToString());
    }

    public Task<IActionResult> Me()
    {
        return Task.FromResult<IActionResult>(View(model: new HomeViewModel()));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    ///     Stop the application
    /// </summary>
    /// <returns></returns>
    [HttpPost("stop-application")]
    public ActionResult StopApplication()
    {
        Task.Factory.StartNew(() =>
        {
            Thread.Sleep(3_000);
            _appLifetime.StopApplication();
        });

        return NoContent();
    }
}
