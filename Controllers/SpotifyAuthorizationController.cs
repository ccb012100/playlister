using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Models;

namespace Playlister.Controllers
{
    [ApiController, Route("api/auth")]
    public class SpotifyAuthorizationController : BaseController
    {
        public SpotifyAuthorizationController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// GetAll the Spotify Accounts URL to direct user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Uri authUrl = await Mediator.Send(new GetAuthUrlCommand());

            return Ok(authUrl);
        }

        /// <summary>
        /// GetAll an Access Token for User.
        /// </summary>
        /// <param name="tokenCommand"></param>
        /// <returns></returns>
        [HttpPost("token")]
        public async Task<IActionResult> AccessToken([FromBody] GetAccessTokenCommand tokenCommand)
        {
            UserAccessToken token = await Mediator.Send(tokenCommand);

            return Ok(token);
        }

        [ValidateToken, HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
        {
            UserAccessToken userAccessToken = await Mediator.Send(refreshTokenCommand);

            return Ok(userAccessToken);
        }
    }
}
