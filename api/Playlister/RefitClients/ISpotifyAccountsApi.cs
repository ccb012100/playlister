using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyAccounts;
using Refit;

namespace Playlister.RefitClients;

public interface ISpotifyAccountsApi
{
    /// <summary>
    ///     Request Access Token for the authenticated User who was granted the provided code from Spotify.
    /// </summary>
    /// <param name="bodyParams"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Post("/api/token")]
    Task<SpotifyAccessToken> RequestAccessTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)] GetAccessTokenCommand.BodyParams bodyParams,
        CancellationToken ct
    );

    /// <summary>
    ///     Get a Refresh Token for User from Spotify.
    /// </summary>
    /// <param name="authHeaderParam"></param>
    /// <param name="bodyParams"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Post("/api/token")]
    Task<SpotifyAccessToken> RefreshTokenAsync(
        [Authorize("Basic")] string authHeaderParam,
        [Body(BodySerializationMethod.UrlEncoded)]
        RefreshTokenCommand.BodyParams bodyParams,
        CancellationToken ct
    );
}
