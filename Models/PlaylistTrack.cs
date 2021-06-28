using System;

namespace Playlister.Models
{
    /// <summary>
    /// Representation of a record in the PlaylistTrack table
    /// </summary>
    public record PlaylistTrack
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public int TrackNumber { get; init; }
        public int DiscNumber { get; init; }
        public DateTime AddedAt { get; init; }
        public int DurationMs { get; init; }
        public string AlbumId { get; init; }
        public string PlaylistId { get; init; }
        public string? SnapshotId { get; init; }
    }
}
