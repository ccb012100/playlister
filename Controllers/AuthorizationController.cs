using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Handlers;
using Playlister.Requests;

namespace Playlister.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthorizationController : BaseController
    {
        public AuthorizationController(IMediator mediator) : base(mediator)
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
            await Mediator.Send(tokenRequest);

            return Ok();
        }
    }
}
