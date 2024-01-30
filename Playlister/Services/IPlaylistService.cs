using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Playlister.CQRS.Commands;
using Playlister.Models;

namespace Playlister.Services
{
    public interface IPlaylistService
    {
        /// <summary>
        /// Update the playlist specified in the <paramref name="command"></paramref> parameter.
        /// </summary>
        Task UpdatePlaylistAsync(UpdatePlaylistCommand command, CancellationToken ct);

        /// <summary>
        /// Update the playlists provided.
        ///
        /// Note: The items in <paramref name="playlists"/> are directly compared to the versions in the database,
        /// so the caller should be providing current versions retrieved from Spotify's API.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="playlists">The playlists to update. These are directly compared to the versions in the database, so the caller should be providing current versions retrieved from Spotify's API.</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UpdatePlaylistsAsync(string accessToken, IEnumerable<Playlist> playlists, CancellationToken ct);

        /// <summary>
        /// The full lists of playlists for the current user.
        /// </summary>
        /// <returns></returns>
        Task<ImmutableArray<Playlist>> GetCurrentUserPlaylistsAsync(string accessToken, CancellationToken ct);

        Task DeleteOrphanedPlaylistTracksAsync(CancellationToken ct);

        /// <summary>
        /// Sync the specified playlist.
        /// This will do a full sync, even if the snapshot Id has not changed since the last update.
        /// </summary>
        /// <param name="playlists"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task SyncPlaylistAsync(string accessToken, string playlistId, CancellationToken ct);
    }
}
