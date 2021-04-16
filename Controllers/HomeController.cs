using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Playlister.Controllers
{
    public class Ping : IRequest<string>
    {
    }

    // ReSharper disable once UnusedType.Global
    public class PingHandler : IRequestHandler<Ping, string>
    {
        public Task<string> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Pong");
        }
    }

    public class SomeEvent : INotification
    {
        public SomeEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    // ReSharper disable once UnusedType.Global
    public class Handler1 : INotificationHandler<SomeEvent>
    {
        private readonly ILogger<Handler1> _logger;

        public Handler1(ILogger<Handler1> logger)
        {
            _logger = logger;
        }

        public Task Handle(SomeEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Handled: {notification.Message}");
            return Task.CompletedTask;
        }
    }

    // ReSharper disable once UnusedType.Global
    public class Handler2 : INotificationHandler<SomeEvent>
    {
        private readonly ILogger<Handler2> _logger;

        public Handler2(ILogger<Handler2> logger)
        {
            _logger = logger;
        }

        public Task Handle(SomeEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Handled: {notification.Message}");
            return Task.CompletedTask;
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _mediator.Publish(new SomeEvent("Hello World"));
            // example of request/response messages
            string result = await _mediator.Send(new Ping());
            return Ok(result);
        }
    }
}
