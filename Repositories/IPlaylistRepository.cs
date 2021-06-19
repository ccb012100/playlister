using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Repositories
{
    public interface IPlaylistRepository
    {
        /// <summary>
        /// Upsert new Playlist entries in the DB.
        /// </summary>
        /// <param name="playlists">Playlists to write to DB.</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task Upsert(IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct);

        /// <summary>
        /// Get all Playlists from the DB
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Playlist>> Get();
    }
}
