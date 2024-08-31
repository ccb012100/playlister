using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.CQRS.Handlers;
using Playlister.Models.SpotifyApi;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ValidateTokenCookie]
[ApiController]
[Route("api/user")]
public class UserController : BaseController
{
    private readonly GetCurrentUserHandler _getCurrentUserHandler;

    public UserController(
        IAccessTokenUtility tokenUtility,
        GetCurrentUserHandler getCurrentUserHandler
    ) : base(tokenUtility)
    {
        _getCurrentUserHandler = getCurrentUserHandler;
    }

    /// <summary>
    ///     Get the User who was assigned the Access Token in the Request <see cref="Playlister.Services.TokenService.UserTokenCookieName" /> cookie.
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    public async Task<PrivateUserObject> GetFromCookie()
    {
        return await _getCurrentUserHandler.Handle(new GetCurrentUserCommand(CookieToken));
    }
}
