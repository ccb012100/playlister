using Playlister.Models.SpotifyAccounts;
using Refit;

namespace Playlister.RefitClients;

/// <summary>
///     Interface for interacting with the Spotify Accounts API
/// </summary>
public interface ISpotifyAccountsApi
{
    /*
     *  The requests to the API will also work if the ClientId and ClientSecret are added as POST body data instead of in the Authorization Header,
     *  but I've chosen to pass them in the Header to match the Spotify Developer Documentation
     *  <https://developer.spotify.com/documentation/web-api/tutorials/code-flow>
     */

    /// <summary>
    ///     Request Access Token for the authenticated User who was granted the provided code from Spotify.
    /// </summary>
    /// <param name="authHeaderParam">
    ///     Base 64 encoded string that contains the client ID and client secret key in the format <c>client_id:client_secret</c>
    ///     The field must have the format: <c>Authorization: Basic base64_encoded_string</c>
    /// </param>
    /// <param name="bodyRequestParams"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Post( "/api/token" )]
    Task<SpotifyAccessToken> RequestAccessTokenAsync(
        [Authorize( "Basic" )] string authHeaderParam,
        [Body( BodySerializationMethod.UrlEncoded )]
        AccessTokenRequestParams bodyRequestParams,
        CancellationToken ct
    );

    /// <summary>
    ///     Get a Refresh Token for User from Spotify.
    /// </summary>
    /// <param name="authHeaderParam">
    ///     Base 64 encoded string that contains the client ID and client secret key in the format <c>client_id:client_secret</c>
    ///     The field must have the format: <c>Authorization: Basic base64_encoded_string</c>
    /// </param>
    /// <param name="bodyParams"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Post( "/api/token" )]
    Task<SpotifyAccessToken> RefreshTokenAsync(
        [Authorize( "Basic" )] string authHeaderParam,
        [Body( BodySerializationMethod.UrlEncoded )]
        TokenRefreshParams bodyParams,
        CancellationToken ct
    );
}
