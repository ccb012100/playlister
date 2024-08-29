using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.CQRS.Commands;
using Playlister.ViewModels;

namespace Playlister.Controllers;

public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator) => _mediator = mediator;

    /// Navigate to Spotify login
    public async Task<IActionResult> Index()
    {
        // TODO: check for auth cookie and if it's already there, just redirect to Sync
        return Redirect((await _mediator.Send(new GetAuthUrlCommand())).ToString());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
