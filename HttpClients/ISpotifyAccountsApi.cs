using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.Spotify;
using Playlister.Requests;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        [Get("/authorize")]
        Task<string> Authorize(AuthQueryParams queryParams, CancellationToken cancellationToken);

        /// <summary>
        /// Request Access Token for the authenticated User who was granted the provided code from Spotify.
        /// </summary>
        /// <param name="requestParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Post("/api/token")]
        Task<AccessInfo> AccessToken([Body(BodySerializationMethod.UrlEncoded)]
            AccessTokenRequestParams requestParams, CancellationToken cancellationToken);

        /// <summary>
        /// Get a Refresh Token for User from Spotify.
        /// </summary>
        /// <param name="authHeaderParam"></param>
        /// <param name="requestParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Post("/api/token")]
        Task<AccessInfo> RefreshToken([Authorize("Basic")] string authHeaderParam,
            [Body(BodySerializationMethod.UrlEncoded)]
            TokenRefreshRequestParams requestParams, CancellationToken cancellationToken);
    }
}
