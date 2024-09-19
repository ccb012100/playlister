using Playlister.CQRS.Queries;
using Playlister.Models.SpotifyAccounts;

namespace Playlister.Services;

public interface IAuthService {
    /// <summary>
    ///     Get URL for authorizing into Spotify.
    /// </summary>
    /// <returns></returns>
    Uri GetSpotifyAuthUrl( );

    /// <summary>
    ///     Get a new <see cref="AuthenticationToken" /> from the <b>Spotify</b> API
    /// </summary>
    /// <param name="auth">The code and state that were returned by <b>Spotify</b> authorization flow</param>
    /// <param name="ct"></param>
    /// <returns>A <see cref="Guid" /> used to access the <see cref="AuthenticationToken" /></returns>
    Task<Guid> GetAccessToken( AuthorizationResult auth , CancellationToken ct = default );

    /// <summary>
    ///     Refresh the current user's token
    /// </summary>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <returns>A <see cref="Guid" /> used to access the <see cref="AuthenticationToken" /></returns>
    Task<Guid> RefreshSpotifyToken( RefreshTokenQuery query , CancellationToken ct = default );
}
