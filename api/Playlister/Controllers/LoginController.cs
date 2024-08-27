using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.ViewModels;

namespace Playlister.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IMediator _mediator;

        public LoginController(ILogger<LoginController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Task<IActionResult> Index([FromQuery] string code, [FromQuery] string state, [FromQuery] string? error)
        {
            _logger.LogDebug("Running: {}.{}", nameof(SyncController), nameof(Index));

            switch (error)
            {
                case null:
                    /* TODO:
                     *  - generate user token tied to Spotify token
                     *  - set user token in cookie
                     *  - redirect to Sync view
                     */
                    return Task.FromResult<IActionResult>(View(new LoginViewModel { State = state, Code = code }));
                default:
                    _logger.LogError("Spotify auth returned an error: {AuthError}", error);

                    throw new InvalidOperationException($"Error authenticating with Spotify: {error}");
            }
        }
    }
}
