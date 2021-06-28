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
        /// GetAll all Playlists from the DB
        /// </summary>
        /// <returns></returns>
        IEnumerable<Playlist> GetAll();

        Playlist? Get(string id);

        Task Upsert(SimplifiedPlaylistObject playlist);
    }
}
