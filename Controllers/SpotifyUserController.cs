using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Models;
using Playlister.Requests;

namespace Playlister.Controllers
{
    [ValidateToken]
    [ApiController]
    [Route("api/user")]
    public class SpotifyUserController : BaseController
    {
        public SpotifyUserController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string authorization)
        {
            SpotifyUserProfile user = await Mediator.Send(new SpotifyUserRequest(authorization));

            return Ok(user);
        }
    }
}
