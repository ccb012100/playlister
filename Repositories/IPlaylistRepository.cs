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
        /// Create playlist.
        /// If playlist exists, update the entry IFF <c>snapshot_id</c> differs.
        /// </summary>
        /// <param name="playlists"></param>
        /// <param name="ct"></param>
        Task Upsert(IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct);

        /// <summary>
        /// Get all Playlists from the DB
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Playlist>> Get();

        Task<Playlist> Get(string id);
    }
}
