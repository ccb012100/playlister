using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyApi
    {
        [Get("/me")]
        Task<string> GetUser([Authorize("Bearer")] string token, CancellationToken cancellationToken);
    }
}
