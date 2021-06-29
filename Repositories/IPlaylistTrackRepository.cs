using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Repositories
{
    public interface IPlaylistTrackRepository
    {
        Task Upsert(Playlist playlist, IEnumerable<PlaylistItem> playlistItems, CancellationToken ct);
    }
}
