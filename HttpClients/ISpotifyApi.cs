using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.Spotify;
using Refit;

namespace Playlister.HttpClients
{
    public interface ISpotifyApi
    {
        /// <summary>
        /// Get detailed profile information about the current user (including the current user’s username).
        /// </summary>
        /// <param name="token">Spotify Access Token</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The User who was assigned the provided Access Token.</returns>
        [Get("/me")]
        Task<PrivateUserObject> GetCurrentUser([Authorize("Bearer")] string token, CancellationToken cancellationToken);
    }
}
