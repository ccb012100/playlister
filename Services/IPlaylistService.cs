using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;

namespace Playlister.Services
{
    public interface IPlaylistService
    {
        Playlist? GetPlaylist(string id);
        Task UpdatePlaylist(Playlist playlist, IEnumerable<PlaylistItem> listItems, CancellationToken ct);
    }
}
