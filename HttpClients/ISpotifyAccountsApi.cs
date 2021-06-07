using System.Threading;
using System.Threading.Tasks;
using Playlister.Requests;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        [Get("/authorize")]
        Task<string> Authorize(AuthQueryParams queryParams, CancellationToken cancellationToken);

        [Post("/api/token")]
        Task<IApiResponse<SpotifyAccessToken>> AccessToken(
            [Body(BodySerializationMethod.UrlEncoded)]
            AccessTokenRequestParams tokenParams,
            CancellationToken cancellationToken);
    }
}
