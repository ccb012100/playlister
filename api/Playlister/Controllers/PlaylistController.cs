using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.DTOs;
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
    public Task<ActionResult> UpdateTracks(string playlistId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Sync the specified playlist
    /// </summary>
    /// <param name="playlistId">ID of the Playlist to update</param>
    /// <returns></returns>
    [HttpPost($"sync/{{{nameof(playlistId)}}}")]
    public Task<ActionResult> SyncPlaylist(string playlistId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Update the current user's playlists
    /// </summary>
    /// <returns></returns>
    [HttpPost("sync")]
    public async Task<ActionResult<SyncResultDto>> SyncAllPlaylists()
    {
        ((int total, int updated, int deleted), TimeSpan elapsed) = await RunInTimer(
            async () =>
                await Mediator.Send(new UpdateCurrentUserPlaylistsCommand(CookieAccessToken))
        );

        string elapsedStr = elapsed.ToDisplayString();

        _logger.LogInformation(
            "Updated {Changed}/{Total} of the current user's playlists. Total time: {Elapsed}",
            updated,
            total,
            elapsedStr
        );

        return Ok(
            new SyncResultDto
            {
                TotalSynced = total,
                Deleted = deleted,
                Elapsed = elapsed.ToDisplayString(),
                Updated = updated
            }
        );
    }
}
