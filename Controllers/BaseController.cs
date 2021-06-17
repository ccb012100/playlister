using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Playlister.Controllers
{
    [ApiController, Route("api/[controller]")]
    public abstract class BaseController : Controller
    {
        internal readonly IMediator Mediator;

        protected BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}