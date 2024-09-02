using System.Text.Json.Serialization;

namespace Playlister.Models;

public record Track
{
    public required Album Album { get; init; }

    public required IEnumerable<Artist> Artists { get; init; }

    [JsonPropertyName( "disc_number" )] public int DiscNumber { get; init; }

    [JsonPropertyName( "duration_ms" )] public int DurationMs { get; init; }

    public required string Id { get; init; }

    public required string Name { get; init; }

    [JsonPropertyName( "track_number" )] public int TrackNumber { get; init; }

    public string AlbumId => Album.Id;

    /// <summary>
    ///     Flatten the track's artists and the track's album's artists into a single collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Artist> GetAllContainedArtists()
    {
        return Artists.Concat( Album.Artists );
    }

    /// <summary>
    ///     Get Track/Artist TrackId pair for each artist on the track.
    /// </summary>
    /// <returns>Collection of track id, artist id tuples</returns>
    public IEnumerable<object> GetArtistIdPairings()
    {
        return Artists.Select(
            a => new
            {
                TrackId = Id,
                ArtistId = a.Id
            }
        );
    }
}
