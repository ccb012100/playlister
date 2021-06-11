using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.Spotify;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyApi
    {
        [Get("/me")]
        Task<UserProfile> GetUser([Authorize("Bearer")] string token, CancellationToken cancellationToken);
    }
}
