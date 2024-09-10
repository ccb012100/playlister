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
}
