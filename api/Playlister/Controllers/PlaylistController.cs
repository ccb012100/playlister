using Microsoft.AspNetCore.Mvc;

using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.CQRS.Handlers;
using Playlister.CQRS.Queries;
using Playlister.Mvc.DTOs;
using Playlister.Utilities;

using static Playlister.CQRS.Handlers.PlaylistSyncHandler;

namespace Playlister.Controllers;

[ValidateTokenCookie]
[ApiController]
[Route( "api/playlists" )]
public class PlaylistController(
    IAccessTokenUtility tokenUtility ,
    ILogger<PlaylistController> logger ,
    CurrentUserHandler currentUserHandler ,
    PlaylistSyncHandler playlistSyncHandler
) : BaseApiController( tokenUtility ) {
    private readonly CurrentUserHandler _currentUserHandler = currentUserHandler;
    private readonly ILogger<PlaylistController> _logger = logger;
    private readonly PlaylistSyncHandler _playlistSyncHandler = playlistSyncHandler;

    /// <summary>
    ///     Get the current user's Playlists.
    /// </summary>
    /// <returns>All Playlists owned by the current user</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists( CancellationToken ct ) {
        ( IEnumerable<Playlist> lists , TimeSpan elapsed ) = await RunInTimer( async ( ) =>
            await _currentUserHandler.GetPlaylists( new GetCurrentUserPlaylistsQuery( CookieToken ) , ct )
        );

        _logger.LogInformation(
            "Retrieved current user's {PlaylistCount} playlists. Total time: {Elapsed}" ,
            lists.Count( ) ,
            elapsed.ToDisplayString( )
        );

        return Ok( lists );
    }

    /// <summary>
    ///     Update the current user's playlists
    /// </summary>
    /// <returns></returns>
    [HttpPost( "sync" )]
    public async Task<ActionResult<SyncResultsDto>> SyncAllPlaylists( CancellationToken ct ) {
        _logger.LogInformation( "Syncing all playlists..." );

        ( SyncResults syncResults , TimeSpan elapsed ) = await RunInTimer( async ( ) =>
            await _playlistSyncHandler.SyncAllForCurrentUser( new SyncCurrentUserPlaylistsCommand( CookieToken ) , ct )
        );

        string elapsedStr = elapsed.ToDisplayString( );

        _logger.LogInformation(
            "Updated {Changed}/{Total} of the current user's playlists. Total time: {Elapsed}" ,
            syncResults.PlaylistsUpdated ,
            syncResults.PlaylistCount ,
            elapsedStr
        );

        return Ok(
            new SyncResultsDto {
                OrphanedTracksDeleted = syncResults.OrphanedTracksDeleted ,
                PlaylistAlbumCount = syncResults.PlaylistAlbumCount ,
                PlaylistCount = syncResults.PlaylistCount ,
                PlaylistsUpdated = syncResults.PlaylistsUpdated ,
                TimeElapsed = elapsed.ToDisplayString( ) ,
            }
        );
    }

    /// <summary>
    ///     Sync the specified playlist
    /// </summary>
    /// <param name="playlistId">ID of the Playlist to update</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost( $"sync/{{{nameof(playlistId)}}}" )]
    public async Task<ActionResult> SyncPlaylist( string playlistId , CancellationToken ct ) {
        await _playlistSyncHandler.SyncSingle( new SyncPlaylistCommand( CookieToken , playlistId ) , ct );

        return NoContent( );
    }

    /// <summary>
    ///     Sync the specified playlist, regardless of whether it's changed since the last snapshot
    /// </summary>
    /// <param name="playlistId">ID of the Playlist to update</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost( $"sync/{{{nameof(playlistId)}}}/force" )]
    public async Task<ActionResult> ForceSyncPlaylist( string playlistId , CancellationToken ct ) {
        await _playlistSyncHandler.ForceSync( new ForceSyncPlaylistCommand( CookieToken , playlistId ) , ct );

        return NoContent( );
    }
}
