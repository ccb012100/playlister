using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Extensions;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ValidateAuthHeaderToken]
[ApiController]
[Route("api/playlists")]
public class PlaylistController : BaseController
{
    private readonly ILogger<PlaylistController> _logger;

    public PlaylistController(IMediator mediator, IAccessTokenUtility tokenUtility, ILogger<PlaylistController> logger)
        : base(mediator, tokenUtility)
    {
        _logger = logger;
    }

    [HttpPost("{playlistId}/tracks")]
    public async Task<ActionResult> UpdateTracks(string playlistId)
    {
        await Mediator.Send(new UpdatePlaylistCommand(AuthHeaderAccessToken, playlistId));

        return NoContent();
    }

    /// <summary>
    /// Sync the specified playlist
    /// </summary>
    /// <param name="playlistId">ID of the Playlist to update</param>
    /// <returns></returns>
    [HttpPost($"sync/{{{nameof(playlistId)}}}")]
    public async Task<ActionResult> SyncPlaylist(string playlistId)
    {
        await Mediator.Send(new SyncPlaylistCommand(AuthHeaderAccessToken, playlistId));

        return NoContent();
    }

    /// <summary>
    ///     Update the current user's playlists
    /// </summary>
    /// <returns></returns>
    [HttpPost("sync")]
    public async Task<ActionResult<(int totalSynced, string elapsed, int updated)>> SyncAllPlaylists()
    {
        ((int total, int updated), TimeSpan elapsed) = await RunInTimer(async () =>
            await Mediator.Send(new UpdateCurrentUserPlaylistsCommand(CookieAccessToken))
        );

        string elapsedStr = elapsed.ToLogString();

        _logger.LogInformation("Updated {Changed}/{Total} of the current user's playlists. Total time: {Elapsed}",
            updated,
            total,
            elapsedStr
        );

        return Ok((total, elapsedStr, updated));
    }
}
