using System.Text.Json.Serialization;

namespace Playlister.Models;

/// <summary>
/// Represents a row in the <see cref="Playlister.Data.DataTables.PlaylistAlbum"/> table.
/// </summary>
public record PlaylistAlbum
{
    /// <summary>
    /// The name of the album
    /// </summary>
    public required string Album { get; init; }

    /// <summary>
    /// All artists on the album
    /// </summary>
    public required string Artists { get; init; }

    /// <summary>
    /// The number of tracks on the album
    /// </summary>
    [JsonPropertyName( "track_count" )] public required int TrackCount { get; init; }


    /// <summary>
    /// The album's release year as a 4-character string
    /// </summary>
    [JsonPropertyName( "release_year" )] public required string ReleaseYear { get; init; }

    /// <summary>
    /// The name of the playlist the album is on
    /// </summary>
    public required string Playlist { get; init; }

    /// <summary>
    /// The date the album was added to <see cref="Playlist"/>
    /// </summary>
    [JsonPropertyName( "added_at" )] public required string AddedAt { get; init; }
}
