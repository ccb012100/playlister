using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.SpotifyApi;
using Refit;

namespace Playlister.RefitClients
{
    public interface ISpotifyApi
    {
        /// <summary>
        /// GetAll detailed profile information about the current user (including the current user’s username).
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>The User who was assigned the provided Access Token.</returns>
        [Get("/me")]
        Task<PrivateUserObject> GetCurrentUser(CancellationToken ct);

        [Get("/playlists/{playlistId}")]
        Task<SimplifiedPlaylistObject> GetPlaylist(string playlistId, CancellationToken ct);
    }
}
