using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Requests;

namespace Playlister.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class SpotifyUserController : BaseController
    {
        public SpotifyUserController(IMediator mediator) : base(mediator)
        {
        }

        [ValidateToken]
        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string authorization)
        {
            string user = await Mediator.Send(new SpotifyUserRequest(authorization.Substring("Bearer ".Length)));

            return Ok(user);
        }
    }
}
