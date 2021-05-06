using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Playlister.Api.Handlers;

namespace Playlister.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StartController : BaseController
    {
        public StartController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [DisableCors]
        public async Task<IActionResult> Get()
        {
            object result = await Mediator.Send(new AuthorizationRequest());

            return Ok(result);
        }
    }
}
