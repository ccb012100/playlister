using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route("api/user")]
public class CurrentUserController : BaseController
{
    private readonly IHostApplicationLifetime _appLifetime;

    public CurrentUserController(
        IMediator mediator,
        IAccessTokenUtility tokenUtility,
        IHostApplicationLifetime appLifetime
    ) : base(mediator, tokenUtility)
    {
        _appLifetime = appLifetime;
    }

    /// <summary>
    ///     Get the User who was assigned the Access Token in the Request <see cref="Playlister.Services.TokenService.UserTokenCookieName" /> cookie.
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    public async Task<PrivateUserObject> GetFromCookie()
    {
        PrivateUserObject user = await Mediator.Send(new GetCurrentUserCommand(CookieAccessToken));

        return user;
    }

    /// <summary>
    ///     Get the current user's Playlists.
    /// </summary>
    /// <returns></returns>
    [ValidateAuthHeaderToken]
    [HttpGet("playlists")]
    [Obsolete($"deprecated; use {nameof(PlaylistController)} instead")]
    public Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
    {
        throw new NotImplementedException();
        /*_logger.LogInformation(
            "Retrieved current user's {PlaylistCount} playlists. Total time: {Elapsed}",
            lists.Count(),
            elapsed.ToDisplayString()
        );*/
    }

    /// <summary>
    ///     Update list of current user's playlists.
    /// </summary>
    /// <returns></returns>
    [Obsolete($"deprecated - use {nameof(HomeController)}.{nameof(HomeController.StopApplication)} instead")]
    [HttpPost("stop-application")]
    public ActionResult StopApplication()
    {
        Task.Factory.StartNew(
            () =>
            {
                Thread.Sleep(3_000);
                _appLifetime.StopApplication();
            }
        );

        return NoContent();
    }
}
