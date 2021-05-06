using System.Threading;
using System.Threading.Tasks;
using Playlister.Api.Handlers;
using Refit;

namespace Playlister.Api.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        [Get("/authorize")]
        Task<string> Authorize(AuthQueryParams queryParams, CancellationToken cancellationToken);
    }
}
