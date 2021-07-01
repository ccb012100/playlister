using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Services
{
    public interface IPlaylistService
    {
        Playlist? GetPlaylist(string id);
        Task UpdatePlaylist(string playlistId, int offset, int limit, CancellationToken ct);
        Task UpdatePlaylists(IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct);
    }
}
