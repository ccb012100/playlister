using System.Text.Json.Serialization;

namespace Playlister.Models;

public record Playlist {
    public required string Id { get; init; }

    [JsonPropertyName( "snapshot_id" )] public string? SnapshotId { get; init; }

    public required string Name { get; init; }

    public bool Collaborative { get; init; }

    public string? Description { get; init; }

    public bool? Public { get; init; }

    /// <summary>
    ///     Number of tracks in the playlist
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    ///     Number of unique tracks in the playlist (i.e. without duplicates)
    /// </summary>
    public int CountUnique { get; init; }

    public string LoggingTag => $"[{ToString( )}]";

    public override string ToString( ) {
        return $"Playlist `{Id}` (\"{Name}\")";
    }
}
