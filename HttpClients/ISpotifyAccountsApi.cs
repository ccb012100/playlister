using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.SpotifyAccounts;
using Playlister.Requests;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        /// <summary>
        /// Request Access Token for the authenticated User who was granted the provided code from Spotify.
        /// </summary>
        /// <param name="bodyParams"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Post("/api/token")]
        Task<AccessInfo> AccessToken([Body(BodySerializationMethod.UrlEncoded)]
            AccessTokenRequest.BodyParams bodyParams, CancellationToken ct);

        /// <summary>
        /// Get a Refresh Token for User from Spotify.
        /// </summary>
        /// <param name="authHeaderParam"></param>
        /// <param name="bodyParams"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Post("/api/token")]
        Task<AccessInfo> RefreshToken([Authorize("Basic")] string authHeaderParam,
            [Body(BodySerializationMethod.UrlEncoded)]
            TokenRefreshRequest.BodyParams bodyParams, CancellationToken ct);
    }
}
