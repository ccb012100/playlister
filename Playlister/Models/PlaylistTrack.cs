using System;

#pragma warning disable 8618

namespace Playlister.Models
{
    /// <summary>
    /// Representation of a record in the PlaylistTrack table
    /// </summary>
    public record PlaylistTrack
    {
        public string TrackId { get; init; }
        public DateTime AddedAt { get; init; }
        public string PlaylistId { get; init; }
        public string? SnapshotId { get; init; }
    }
}
