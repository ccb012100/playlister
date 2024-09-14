namespace Playlister.Repositories;

public interface IPlaylistWriteRepository
{
    /// <summary>Add or update the supplied playlist items to the specified playlist.</summary>
    /// <param name="playlist"></param>
    /// <param name="playlistItems"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task UpsertAsync( Playlist playlist, IEnumerable<PlaylistItem> playlistItems, CancellationToken ct );

    /// <summary>Delete tracks that are not linked to any playlists</summary>
    /// <param name="ct"></param>
    /// <returns>Total number of tracks deleted</returns>
    Task<int> DeleteOrphanedPlaylistTracksAsync( CancellationToken ct );

    /// <summary>
    ///     Truncate <see cref="Data.DataTables.PlaylistAlbum"/> and then populate it from scratch.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>The number of items added to the table, i.e. the total number of rows in the table</returns>
    /// <remarks>
    ///     Currently this is performant enough that it's preferable to trying to keep it in sync
    ///     with the canonical data by reconciling existing entries.
    /// </remarks>
    Task<int> TruncateAndPopulatePlaylistAlbum( CancellationToken ct );
}
