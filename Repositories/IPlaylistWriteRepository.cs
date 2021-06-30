using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Repositories
{
    public interface IPlaylistWriteRepository
    {
        /// <summary>
        /// Create playlist.
        /// If playlist exists, update the entry IFF <c>snapshot_id</c> differs.
        /// </summary>
        /// <param name="playlists"></param>
        /// <param name="ct"></param>
        Task Upsert(IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct);

        Task Upsert(Playlist playlist, IEnumerable<PlaylistItem> playlistItems, CancellationToken ct);
    }
}
