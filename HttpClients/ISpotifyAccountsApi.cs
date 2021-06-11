using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.Spotify;
using Playlister.Requests;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        [Get("/authorize")]
        Task<string> Authorize(AuthQueryParams queryParams, CancellationToken cancellationToken);

        [Post("/api/token")]
        Task<AccessInfo> AccessToken([Body(BodySerializationMethod.UrlEncoded)]
            AccessTokenRequestParams requestParams, CancellationToken cancellationToken);

        [Post("/api/token")]
        Task<AccessInfo> RefreshToken([Authorize("Basic")] string authHeaderParam,
            [Body(BodySerializationMethod.UrlEncoded)]
            TokenRefreshRequestParams requestParams, CancellationToken cancellationToken);
    }
}
