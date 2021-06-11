using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
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
            UserAccessInfo userInfo = await Mediator.Send(tokenRequest);

            return Ok(userInfo);
        }

        [ValidateToken]
        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest tokenRefreshRequest)
        {
            UserAccessInfo userAccessInfo = await Mediator.Send(tokenRefreshRequest);

            return Ok(userAccessInfo);
        }
    }
}
