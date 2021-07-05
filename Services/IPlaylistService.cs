using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;

namespace Playlister.Services
{
    public interface IPlaylistService
    {
        Task UpdatePlaylist(string accessToken, string playlistId, int offset, int limit, CancellationToken ct);
        Task UpdatePlaylist(UpdatePlaylistCommand updateCommand, CancellationToken ct);
        Task UpdatePlaylists(string accessToken, IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct);
    }
}
