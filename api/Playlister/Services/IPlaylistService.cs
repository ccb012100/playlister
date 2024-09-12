namespace Playlister.Services;

public interface IPlaylistService
{
    /// <summary>
    ///     Update the playlists provided.<br /><br />
    ///     <b>Note:</b> The items in <paramref name="playlists" /> are directly compared to the versions in the database,
    ///     so the caller should be providing current versions retrieved from Spotify's API.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="playlists">
    ///     The playlists to update. These are directly compared to the versions in the database, so the caller should be providing
    ///     current versions retrieved from Spotify's API.
    /// </param>
    /// <param name="ct"></param>
    /// <returns>The number of playlists updated.</returns>
    Task<int> SyncPlaylistsAsync( string accessToken, IEnumerable<Playlist> playlists, CancellationToken ct );

    /// <summary>
    ///     The full lists of playlists for the current user associated with the supplied Access Token.
    /// </summary>
    Task<ImmutableArray<Playlist>> GetUserPlaylistsAsync( string accessToken, CancellationToken ct );

    /// <summary>
    ///     Sync the specified playlist.
    /// </summary>
    Task SyncPlaylistAsync( string accessToken, string playlistId, CancellationToken ct );

    /// <summary>
    ///     Sync the specified playlist, regardless of whether the snapshot ID has changed since the last update.
    /// </summary>
    Task ForceSyncPlaylistAsync( string accessToken, string playlistId, CancellationToken ct );

    /// <summary>
    ///     Delete tracks without any Playlist associations
    /// </summary>
    /// <remarks>Total number of tracks deleted</remarks>
    Task<int> DeleteOrphanedPlaylistTracksAsync( CancellationToken ct );

    /// <summary>
    ///     Truncate <see cref="Data.DataTables.PlaylistAlbum"/> and then populate it from scratch.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>Number of items deleted and number of items added to the table</returns>
    /// <remarks>
    ///     Currently this is performant enough that it's preferable to trying to keep it in sync
    ///     with the canonical data by reconciling existing entries.
    /// </remarks>
    Task<(int inserted, int deleted)> RebuildPlaylistAlbumTable( CancellationToken ct );
}
