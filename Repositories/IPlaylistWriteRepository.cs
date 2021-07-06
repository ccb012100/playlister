using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories
{
    public interface IPlaylistWriteRepository
    {
        Task Upsert(Playlist playlist, IEnumerable<PlaylistItem> playlistItems, CancellationToken ct);
    }
}
