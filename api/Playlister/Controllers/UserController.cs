using Microsoft.AspNetCore.Mvc;

using Playlister.Attributes;
using Playlister.CQRS.Handlers;
using Playlister.CQRS.Queries;
using Playlister.Models.SpotifyApi;
using Playlister.Services.Implementations;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ValidateTokenCookie]
[ApiController]
[Route( "api/user" )]
public class UserController(
    IAccessTokenUtility tokenUtility ,
    CurrentUserHandler currentUserHandler
) : BaseApiController( tokenUtility ) {
    private readonly CurrentUserHandler _currentUserHandler = currentUserHandler;

    /// <summary>
    ///     Get the User who was assigned the Access Token in the Request <see cref="TokenService.UserTokenCookieName" /> cookie.
    /// </summary>
    /// <returns></returns>
    [HttpGet( "me" )]
    public async Task<PrivateUserObject> GetFromCookie( ) {
        return await _currentUserHandler.Get( new GetCurrentUserQuery( CookieToken ) );
    }
}
