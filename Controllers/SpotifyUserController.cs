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

        /// <summary>
        /// Get the User who was assigned the Access Token in the Authorization Header.
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PrivateUserObject>> Get([FromHeader] string authorization)
        {
            PrivateUserObject user = await Mediator.Send(new CurrentSpotifyUserRequest(authorization));

            return Ok(user);
        }
    }
}
