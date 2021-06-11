using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.Spotify;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyApi
    {
        [Get("/me")]
        Task<PublicUserObject> GetUser([Authorize("Bearer")] string token, CancellationToken cancellationToken);
    }
}
