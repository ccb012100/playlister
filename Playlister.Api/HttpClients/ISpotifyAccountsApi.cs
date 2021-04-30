using System;
using System.Threading.Tasks;
using Refit;

namespace Playlister.Api.HttpClients
{
    public interface ISpotifyAccountsApi
    {
        [Get(
            "/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope={scope}&state={state}&showDialog={showDialog}")]
        Task<object> Authorize(string clientId, Uri redirectUri, string state, string? scope, bool showDialog);
    }
}
