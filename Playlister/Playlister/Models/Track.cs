using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;


#pragma warning disable 8618

namespace Playlister.Models
{
    public record Track
    {
        public Album Album { get; init; }

        public IEnumerable<Artist> Artists { get; init; }

        [JsonPropertyName("disc_number")]
        public int DiscNumber { get; init; }

        [JsonPropertyName("duration_ms")]
        public int DurationMs { get; init; }

        public string Id { get; init; }

        public string Name { get; init; }

        [JsonPropertyName("track_number")]
        public int TrackNumber { get; init; }

        /// <summary>
        /// Flatten the track's artists and the track's album's artists into a single collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Artist> GetAllContainedArtists() => Artists.Concat(Album.Artists);

        /// <summary>
        /// Get Track/Artist TrackId pair for each artist on the track.
        /// </summary>
        /// <returns>Collection of track id, artist id tuples</returns>
        public IEnumerable<object> GetArtistIdPairings() =>
            Artists.Select(a => new { TrackId = Id, ArtistId = a.Id });

        public string AlbumId => Album.Id;
    }
}
