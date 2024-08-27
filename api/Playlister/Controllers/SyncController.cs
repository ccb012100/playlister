using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Playlister.Controllers
{
    public class SyncController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SyncController> _logger;

        public SyncController(IMediator mediator, ILogger<SyncController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task<IActionResult> Index() => Task.FromResult<IActionResult>(View());
    }
}
