using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route("api/auth")]
public class SpotifyAuthorizationController : BaseController
{
    public SpotifyAuthorizationController(IMediator mediator, IAccessTokenUtility tokenUtility) : base(mediator, tokenUtility) { }

    /// <summary>
    ///     Get the Spotify Accounts URL to direct user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Uri authUrl = await _mediator.Send(new GetAuthUrlCommand());

        return Ok(authUrl);
    }

    /// <summary>
    ///     Get an Access Token for User.
    /// </summary>
    /// <param name="tokenCommand"></param>
    /// <returns></returns>
    [HttpPost("token")]
    public async Task<IActionResult> GetAccessToken([FromBody] GetAccessTokenCommand tokenCommand)
    {
        UserAccessToken token = await _mediator.Send(tokenCommand);

        return Ok(token);
    }

    [ValidateAuthHeaderToken]
    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        UserAccessToken userAccessToken = await _mediator.Send(refreshTokenCommand);

        return Ok(userAccessToken);
    }
}