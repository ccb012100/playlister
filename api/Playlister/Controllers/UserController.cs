using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ValidateTokenCookie]
[ApiController]
[Route("api/user")]
public class UserController : BaseController
{
    public const string Name = "User";

    private readonly ILogger<UserController> _logger;

    public UserController(
        ILogger<UserController> logger,
        IMediator mediator,
        IAccessTokenUtility tokenUtility
    ) : base(mediator, tokenUtility)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Get the User who was assigned the Access Token in the Request <see cref="Playlister.Services.TokenService.UserTokenCookieName" /> cookie.
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    public async Task<PrivateUserObject> GetFromCookie()
    {
        PrivateUserObject user = await Mediator.Send(new GetCurrentUserCommand(CookieToken));

        return user;
    }
}
