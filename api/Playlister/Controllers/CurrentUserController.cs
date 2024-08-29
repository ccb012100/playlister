using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Services;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route("api/user")]
public class CurrentUserController : BaseController
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IPlaylistService _playlistService;
    private readonly ILogger<CurrentUserController> _logger;

    public CurrentUserController(
        IMediator mediator,
        IAccessTokenUtility tokenUtility,
        ILogger<CurrentUserController> logger,
        IHostApplicationLifetime appLifetime,
        IPlaylistService playlistService
    ) : base(mediator, tokenUtility)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _playlistService = playlistService;
    }

    /// <summary>
    ///     Get the User who was assigned the Access Token in the Authorization Header.
    /// </summary>
    /// <returns></returns>
    [ValidateAuthHeaderToken]
    [HttpGet]
    public async Task<PrivateUserObject> Get()
    {
        PrivateUserObject user = await _mediator.Send(new GetCurrentUserCommand(AuthHeaderAccessToken));

        return user;
    }

    /// <summary>
    ///     Get the current user's Playlists.
    /// </summary>
    /// <returns></returns>
    [ValidateAuthHeaderToken]
    [HttpGet("playlists")]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
    {
        Stopwatch sw = new();
        sw.Start();

        IEnumerable<Playlist> lists = await _mediator.Send(
            new GetCurrentUserPlaylistsCommand(AuthHeaderAccessToken)
        );

        sw.Stop();

        _logger.LogInformation(
            "Retrieved current user's {PlaylistCount} playlists. Total time: {Elapsed}",
            lists.Count(),
            sw.Elapsed.ToLogString()
        );

        return Ok(lists);
    }

    /// <summary>
    ///     Update list of current user's playlists.
    /// </summary>
    /// <returns></returns>
    [ValidateAuthHeaderToken]
    [HttpPost("playlists")]
    public async Task<ActionResult> UpdateCurrentUserPlaylists()
    {
        Stopwatch sw = new();
        sw.Start();

        (int total, int updated) = await _mediator.Send(new UpdateCurrentUserPlaylistsCommand(AuthHeaderAccessToken));

        sw.Stop();

        _logger.LogInformation(
            "Updated {Changed}/{Total} of the current user's playlists. Total time: {Elapsed}",
            updated,
            total,
            sw.Elapsed.ToLogString()
        );

        return NoContent();
    }

    /// <summary>
    ///     Update list of current user's playlists.
    /// </summary>
    /// <returns></returns>
    [HttpPost("playlists/sync")]
    public async Task<ActionResult<(int totalSynced, string elapsed, int updated)>> SyncAllPlaylists()
    {
        Stopwatch sw = new();
        sw.Start();

        (int total, int updated) = await _mediator.Send(new UpdateCurrentUserPlaylistsCommand(CookieAccessToken));

        sw.Stop();

        string elapsed = sw.Elapsed.ToLogString();

        _logger.LogInformation("Updated {Changed}/{Total} of the current user's playlists. Total time: {Elapsed}", updated, total, elapsed);

        return Ok((total, elapsed, updated));
    }

    /// <summary>
    ///     Update list of current user's playlists.
    /// </summary>
    /// <returns></returns>
    // TODO: move this to a more appropriate controller
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
