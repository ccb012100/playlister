using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Requests;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        [Get("/authorize")]
        Task<string> Authorize(AuthQueryParams queryParams, CancellationToken cancellationToken);

        [Post("/api/token")]
        Task<SpotifyAccessToken> AccessToken([Body(BodySerializationMethod.UrlEncoded)]
            AccessTokenRequestParams requestParams, CancellationToken cancellationToken);

        [Post("/api/token")]
        Task<SpotifyAccessToken> RefreshToken([Authorize("Basic")] string authHeaderParam,
            [Body(BodySerializationMethod.UrlEncoded)]
            TokenRefreshRequestParams requestParams, CancellationToken cancellationToken);
    }
}
