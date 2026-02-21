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
    ///     Number of tracks in the playlist.
    /// </summary>
    /// <remarks>
    ///    WARNING: Adding a duplicate track will sometimes fail to increment this number.
    /// </remarks>
    public int Count { get; init; }

    /// <summary>
    /// TODO: remove this from the database. The Playlist.Count returned from Spotify _is_ the count of unique tracks.
    /// </summary>
    public int CountUnique { get; init; }

    public string LoggingTag => $"[{ToString( )}]";

    public override string ToString( ) {
        return $"Playlist `{Id}` (\"{Name}\")";
    }
}
