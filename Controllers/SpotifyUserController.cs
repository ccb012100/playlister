using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.Models.Spotify;
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
            PublicUserObject user = await Mediator.Send(new SpotifyUserRequest(authorization));

            return Ok(user);
        }
    }
}
