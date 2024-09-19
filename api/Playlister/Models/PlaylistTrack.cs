namespace Playlister.Models;

/// <summary>
///     Representation of a record in the PlaylistTrack table
/// </summary>
public record PlaylistTrack {
    public required string TrackId { get; init; }
    public DateTime AddedAt { get; init; }
    public required string PlaylistId { get; init; }
    public string? SnapshotId { get; init; }
}
