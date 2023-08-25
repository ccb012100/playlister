using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;


#pragma warning disable 8618

namespace Playlister.Models
{
    public record Album
    {
        [JsonPropertyName("album_type")]
        public string AlbumType { get; init; }

        public IEnumerable<Artist> Artists { get; init; }

        public string Id { get; init; }

        public string Name { get; init; }

        /// <summary>
        /// The date the album was first released, for example “1981-12-15”. Depending on the precision, it might be shown as “1981” or “1981-12”.
        /// </summary>
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; init; }

        [JsonPropertyName("release_date_precision")]
        public string ReleaseDatePrecision { get; init; }

        [JsonPropertyName("total_tracks")]
        public int TotalTracks { get; init; }

        [JsonPropertyName("date_of_release")]
        public DateTime DateOfRelease
        {
            get
            {
                // Depending on precision, the ReleaseDate string could be in the format "yyyy", "yyyy-MM" or "yyyy-MM-DD"
                if (
                    DateTime.TryParseExact(ReleaseDate, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime yyyy)
                )
                {
                    return yyyy;
                }

                if (DateTime.TryParse(ReleaseDate, out DateTime dt))
                {
                    return dt;
                }

                throw new InvalidOperationException(
                    $"Could not parse ReleaseDate value {ReleaseDate}"
                );
            }
        }

        /// <summary>
        /// Get a pairing of each artist's TrackId with the album's TrackId.
        /// </summary>
        /// <returns>Collection of album id, artist id tuples.</returns>
        public IEnumerable<object> GetAlbumArtistPairings() =>
            Artists.Select(x => new { AlbumId = Id, ArtistId = x.Id });
    }
}
