using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;

namespace Playlister.Services
{
    public interface IPlaylistService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="playlistId"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UpdatePlaylistAsync(string accessToken, string playlistId, int offset, int limit, CancellationToken ct);

        /// <summary>
        ///
        /// </summary>
        /// <param name="updateCommand"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UpdatePlaylistAsync(UpdatePlaylistCommand updateCommand, CancellationToken ct);

        /// <summary>
        ///
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="playlists"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UpdatePlaylistsAsync(string accessToken, IEnumerable<SimplifiedPlaylistObject> playlists,
            CancellationToken ct);
    }
}
