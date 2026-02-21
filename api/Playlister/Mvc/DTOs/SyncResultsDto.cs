namespace Playlister.Mvc.DTOs;

/// <summary>
/// Represents the results of syncing all Playlists
/// </summary>
public record SyncResultsDto
{
    /// <summary>
    /// The count of orphaned tracks deleted during the sync
    /// </summary>
    public required int OrphanedTracksDeleted { get; init; }

    /// <summary>
    /// The total count of Playlist Albums
    /// </summary>
    public required int PlaylistAlbumCount { get; init; }

    /// <summary>
    /// The total count of Playlists
    /// </summary>
    public required int PlaylistCount { get; init; }

    /// <summary>
    /// The count of Playlists updated during the sync
    /// </summary>
    public required int PlaylistsUpdated { get; init; }

    /// String representation of the time it took to perform the sync
    public required string TimeElapsed { get; init; }
}
