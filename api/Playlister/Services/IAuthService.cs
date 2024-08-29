namespace Playlister.Services;

public interface IAuthService
{
    /// <summary>
    ///     Get URL for authorizing into Spotify.
    /// </summary>
    /// <returns></returns>
    Uri GetSpotifyAuthUrl();

    /// <summary>
    ///     Get token for mapping to a <see cref="Playlister.Models.UserAccessToken" />
    /// </summary>
    /// <param name="code">Code returned from the Spotify API</param>
    /// <param name="ct"></param>
    /// <returns>A <see cref="Guid" /> used to access the <see cref="Playlister.Models.UserAccessToken" /></returns>
    Task<Guid> GetAccessToken(string code, CancellationToken ct = default);
}
