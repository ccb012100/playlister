using System;
using System.Threading.Tasks;
using Playlister.Api.Services;
using Refit;

namespace Playlister.Api.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        [Get("/authorize")]
        Task<object> Authorize(AuthQueryParams queryParams);
    }
}
