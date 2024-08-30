using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route("api/user")]
public class CurrentUserController : BaseController
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<CurrentUserController> _logger;

    public CurrentUserController(
        IMediator mediator,
        IAccessTokenUtility tokenUtility,
        ILogger<CurrentUserController> logger,
        IHostApplicationLifetime appLifetime
    ) : base(mediator, tokenUtility)
    {
        _logger = logger;
        _appLifetime = appLifetime;
    }

    /// <summary>
    ///     Get the User who was assigned the Access Token in the Authorization Header.
    /// </summary>
    /// <returns></returns>
    [ValidateAuthHeaderToken]
    [HttpGet]
    [Obsolete($"deprecated; use {nameof(GetFromCookie)} instead")]
    public async Task<PrivateUserObject> Get()
    {
        PrivateUserObject user = await Mediator.Send(new GetCurrentUserCommand(AuthHeaderAccessToken));

        return user;
    }

    /// <summary>
    ///     Get the User who was assigned the Access Token in the Request <see cref="Playlister.Services.TokenService.UserTokenCookieName"/> cookie.
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
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
    {
        (IEnumerable<Playlist> lists, TimeSpan elapsed) = await RunInTimer(async () =>
            await Mediator.Send(new GetCurrentUserPlaylistsCommand(AuthHeaderAccessToken))
        );

        _logger.LogInformation(
            "Retrieved current user's {PlaylistCount} playlists. Total time: {Elapsed}",
            lists.Count(),
            elapsed.ToDisplayString()
        );

        return Ok(lists);
    }

    /// <summary>
    ///     Update list of current user's playlists.
    /// </summary>
    /// <returns></returns>
    [ValidateAuthHeaderToken]
    [HttpPost("playlists")]
    [Obsolete($"deprecated; use {nameof(PlaylistController)}.{nameof(PlaylistController.SyncAllPlaylists)} instead")]
    public async Task<ActionResult> UpdateCurrentUserPlaylists()
    {
        Stopwatch sw = new();
        sw.Start();

        (int total, int updated) = await Mediator.Send(new UpdateCurrentUserPlaylistsCommand(AuthHeaderAccessToken));

        sw.Stop();

        _logger.LogInformation(
            "Updated {Changed}/{Total} of the current user's playlists. Total time: {Elapsed}",
            updated,
            total,
            sw.Elapsed.ToDisplayString()
        );

        return NoContent();
    }

    /// <summary>
    ///     Update list of current user's playlists.
    /// </summary>
    /// <returns></returns>
    [Obsolete($"deprecated - use {nameof(HomeController)}.{nameof(HomeController.StopApplication)} instead")]
    [HttpPost("stop-application")]
    public ActionResult StopApplication()
    {
        Task.Factory.StartNew(() =>
        {
            Thread.Sleep(3_000);
            _appLifetime.StopApplication();
        });

        return NoContent();
    }
}
