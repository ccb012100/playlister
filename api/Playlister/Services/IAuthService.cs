using Playlister.Models;

namespace Playlister.Services;

public interface IAuthService
{
    /// <summary>
    ///     Get URL for authorizing into Spotify.
    /// </summary>
    /// <returns></returns>
    Uri GetSpotifyAuthUrl();

    /// <summary>
    ///     Get token for mapping to a <see cref="AuthenticationToken" />
    /// </summary>
    /// <param name="auth">The code and state that were returned by <b>Spotify</b> authorization flow</param>
    /// <param name="ct"></param>
    /// <returns>A <see cref="Guid" /> used to access the <see cref="AuthenticationToken" /></returns>
    Task<Guid> GetAccessToken( AuthorizationResult auth, CancellationToken ct = default );
}
