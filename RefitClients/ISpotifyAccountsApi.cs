using System.Threading;
using System.Threading.Tasks;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyAccounts;
using Refit;

namespace Playlister.RefitClients
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
        Task<SpotifyAccessToken> AccessToken([Body(BodySerializationMethod.UrlEncoded)]
            GetAccessTokenCommand.BodyParams bodyParams, CancellationToken ct);

        /// <summary>
        /// GetAll a Refresh Token for User from Spotify.
        /// </summary>
        /// <param name="authHeaderParam"></param>
        /// <param name="bodyParams"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Post("/api/token")]
        Task<SpotifyAccessToken> RefreshToken([Authorize("Basic")] string authHeaderParam,
            [Body(BodySerializationMethod.UrlEncoded)]
            RefreshTokenCommand.BodyParams bodyParams, CancellationToken ct);
    }
}
