using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Repositories
{
    public interface IPlaylistTrackRepository
    {
        Task Upsert(MinimalPlaylist playlist, IEnumerable<PlaylistItem> playlistItems, CancellationToken ct);
    }
}
