using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Handlers;
using Playlister.CQRS.Queries;
using Playlister.Models;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route( "api/auth" )]
public class SpotifyAuthorizationController : BaseApiController
{
    private readonly SpotifyAuthUrlHandler _spotifyAuthUrlHandler;
    private readonly SpotifyTokenRefreshHandler _spotifyTokenRefreshHandler;
    private readonly SpotifyAccessTokenHandler _tokenHandler;

    public SpotifyAuthorizationController(
        IAccessTokenUtility tokenUtility,
        SpotifyAuthUrlHandler spotifyAuthUrlHandler,
        SpotifyAccessTokenHandler tokenHandler,
        SpotifyTokenRefreshHandler spotifyTokenRefreshHandler
    ) : base
        ( tokenUtility )
    {
        _spotifyAuthUrlHandler = spotifyAuthUrlHandler;
        _tokenHandler = tokenHandler;
        _spotifyTokenRefreshHandler = spotifyTokenRefreshHandler;
    }

    /// <summary>
    ///     Get the Spotify Accounts URL to direct user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Uri authUrl = await _spotifyAuthUrlHandler.Handle( new GetAuthUrlQuery() );

        return Ok( authUrl );
    }

    /// <summary>
    ///     Get an Access Token for User.
    /// </summary>
    /// <param name="tokenQuery"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost( "token" )]
    public async Task<IActionResult> GetAccessToken( [FromBody] GetAccessTokenQuery tokenQuery, CancellationToken ct = default )
    {
        AuthenticationToken token = await _tokenHandler.Handle( tokenQuery, ct );

        return Ok( token );
    }

    [ValidateTokenCookie]
    [HttpPost( "token/refresh" )]
    public async Task<IActionResult> RefreshToken( [FromBody] RefreshTokenQuery refreshTokenQuery, CancellationToken ct )
    {
        AuthenticationToken authenticationToken = await _spotifyTokenRefreshHandler.Handle( refreshTokenQuery, ct );

        return Ok( authenticationToken );
    }
}
