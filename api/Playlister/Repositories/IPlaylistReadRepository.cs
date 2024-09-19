namespace Playlister.Repositories;

public interface IPlaylistReadRepository {
    /// <summary>
    ///     Get all playlists from database.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Playlist>> GetAllAsync( );

    /// <summary>
    ///     Get Playlists whose PlaylistTrack count is less than their <see cref="Playlist.Count" /> property.
    /// </summary>
    /// <returns>List of Playlist Ids</returns>
    Task<IEnumerable<(string, int)>> GetPlaylistsWithMissingTracksAsync( );
}
