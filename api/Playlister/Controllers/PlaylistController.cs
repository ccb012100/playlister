using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.CQRS.Handlers;
using Playlister.DTOs;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ValidateTokenCookie]
[ApiController]
[Route("api/playlists")]
public class PlaylistController : BaseController
{
    private readonly ForceSyncPlaylistHandler _forceSyncPlaylistHandler;
    private readonly GetCurrentUserPlaylistsHandler _getCurrentUserPlaylistsHandler;
    private readonly ILogger<PlaylistController> _logger;
    private readonly SyncCurrentUserPlaylistsHandler _syncCurrentUserPlaylistsHandler;
    private readonly SyncPlaylistHandler _syncPlaylistHandler;

    public PlaylistController(
        IAccessTokenUtility tokenUtility,
        ILogger<PlaylistController> logger,
        GetCurrentUserPlaylistsHandler getCurrentUserPlaylistsHandler,
        SyncCurrentUserPlaylistsHandler syncCurrentUserPlaylistsHandler,
        SyncPlaylistHandler syncPlaylistHandler,
        ForceSyncPlaylistHandler forceSyncPlaylistHandler
    )
        : base(tokenUtility)
    {
        _logger = logger;
        _getCurrentUserPlaylistsHandler = getCurrentUserPlaylistsHandler;
        _syncCurrentUserPlaylistsHandler = syncCurrentUserPlaylistsHandler;
        _syncPlaylistHandler = syncPlaylistHandler;
        _forceSyncPlaylistHandler = forceSyncPlaylistHandler;
    }

    /// <summary>
    ///     Get the current user's Playlists.
    /// </summary>
    /// <returns>All Playlists owned by the current user</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
    {
        (IEnumerable<Playlist> lists, TimeSpan elapsed) = await RunInTimer(
            async () =>
                await _getCurrentUserPlaylistsHandler.Handle(new GetCurrentUserPlaylistsCommand(CookieToken))
        );

        _logger.LogInformation(
            "Retrieved current user's {PlaylistCount} playlists. Total time: {Elapsed}",
            lists.Count(),
            elapsed.ToDisplayString()
        );

        return Ok(lists);
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
                await _syncCurrentUserPlaylistsHandler.Handle(new SyncCurrentUserPlaylistsCommand(CookieToken))
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

    /// <summary>
    ///     Sync the specified playlist
    /// </summary>
    /// <param name="playlistId">ID of the Playlist to update</param>
    /// <returns></returns>
    [HttpPost($"sync/{{{nameof(playlistId)}}}")]
    public async Task<ActionResult> SyncPlaylist(string playlistId)
    {
        await _syncPlaylistHandler.Handle(new SyncPlaylistCommand(CookieToken, playlistId));

        return NoContent();
    }

    /// <summary>
    ///     Sync the specified playlist, regardless of wether it's changed since the last snapshot
    /// </summary>
    /// <param name="playlistId">ID of the Playlist to update</param>
    /// <returns></returns>
    [HttpPost($"sync/{{{nameof(playlistId)}}}/force")]
    public async Task<ActionResult> ForceSyncPlaylist(string playlistId)
    {
        await _forceSyncPlaylistHandler.Handle(new ForceSyncPlaylistCommand(CookieToken, playlistId));

        return NoContent();
    }
}
