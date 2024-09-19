using Microsoft.AspNetCore.Mvc;

using Playlister.Attributes;
using Playlister.CQRS.Handlers;
using Playlister.CQRS.Queries;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route( "api/auth" )]
public class SpotifyAuthorizationController(
    IAccessTokenUtility tokenUtility ,
    SpotifyAuthorizationHandler spotifyAuthorizationHandler
    ) : BaseApiController( tokenUtility ) {
    private readonly SpotifyAuthorizationHandler _spotifyAuthorizationHandler = spotifyAuthorizationHandler;

    /// <summary>
    ///     Get the Spotify Accounts URL to direct user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<Uri>( 200 )]
    public async Task<IActionResult> Get( ) {
        return Ok( await _spotifyAuthorizationHandler.GetAuthorizationUrl( ) );
    }

    /// <summary>
    ///     Get an Access Token for User.
    /// </summary>
    /// <param name="tokenQuery"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost( "token" )]
    [ProducesResponseType<Guid>( 200 )]
    public async Task<IActionResult> GetAccessToken( [FromBody] GetAccessTokenQuery tokenQuery , CancellationToken ct = default ) {
        return Ok( await _spotifyAuthorizationHandler.GetSpotifyAccessToken( tokenQuery , ct ) );
    }

    [ValidateTokenCookie]
    [HttpPost( "token/refresh" )]
    [ProducesResponseType<Guid>( 200 )]
    public async Task<IActionResult> RefreshToken( [FromBody] RefreshTokenQuery refreshTokenQuery , CancellationToken ct ) {
        return Ok( await _spotifyAuthorizationHandler.RefreshToken( refreshTokenQuery , ct ) );
    }
}
