using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Models;
using Playlister.Requests;

namespace Playlister.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class SpotifyAuthorizationController : BaseController
    {
        public SpotifyAuthorizationController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Uri authUrl = await Mediator.Send(new AuthUrlRequest());

            return Ok(authUrl);
        }

        [HttpPost("token")]
        public async Task<IActionResult> AccessToken([FromBody] AccessTokenRequest tokenRequest)
        {
            UserAccessToken userToken = await Mediator.Send(tokenRequest);

            return Ok(userToken);
        }

        [ValidateToken]
        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest tokenRefreshRequest)
        {
            UserAccessToken userAccessToken = await Mediator.Send(tokenRefreshRequest);

            return Ok(userAccessToken);
        }
    }
}
