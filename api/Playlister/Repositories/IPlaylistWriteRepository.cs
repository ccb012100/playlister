using Playlister.Models;

namespace Playlister.Repositories;

public interface IPlaylistWriteRepository
{
    /// <summary>
    /// </summary>
    /// <param name="playlist"></param>
    /// <param name="playlistItems"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task UpsertAsync(Playlist playlist, IEnumerable<PlaylistItem> playlistItems, CancellationToken ct);

    /// <summary>
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task DeleteOrphanedPlaylistTracksAsync(CancellationToken ct);
}