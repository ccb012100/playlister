using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyApi
    {
        [Get("/me")]
        Task<SpotifyUserProfile> GetUser([Authorize("Bearer")] string token, CancellationToken cancellationToken);
    }
}
