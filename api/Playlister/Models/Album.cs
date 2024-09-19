using System.Globalization;
using System.Text.Json.Serialization;

namespace Playlister.Models;

public record Album {
    [JsonPropertyName( "album_type" )] public required string AlbumType { get; init; }

    public required IEnumerable<Artist> Artists { get; init; }

    public required string Id { get; init; }

    public required string Name { get; init; }

    /// <summary>
    ///     The date the album was first released, for example “1981-12-15”. Depending on the precision, it might be shown as “1981” or “1981-12”.
    /// </summary>
    [JsonPropertyName( "release_date" )]
    public required string ReleaseDate { get; init; }

    [JsonPropertyName( "release_date_precision" )]
    public required string ReleaseDatePrecision { get; init; }

    [JsonPropertyName( "total_tracks" )] public required int TotalTracks { get; init; }

    [JsonPropertyName( "date_of_release" )]
    public DateTime DateOfRelease {
        get {
            // Depending on precision, the ReleaseDate string could be in the format "yyyy", "yyyy-MM" or "yyyy-MM-DD"
            if (
                DateTime.TryParseExact( ReleaseDate , "yyyy" , CultureInfo.InvariantCulture , DateTimeStyles.None , out DateTime yyyy )
            ) {
                return yyyy;
            }

            if ( DateTime.TryParse( ReleaseDate , out DateTime dt ) ) {
                return dt;
            }

            throw new InvalidOperationException(
                $"Could not parse ReleaseDate value {ReleaseDate}"
            );
        }
    }

    /// <summary>
    ///     Gets a pairing of the Album's ID with <see cref="Album.Artists" />'s Id
    /// </summary>
    /// <returns><see cref="AlbumArtistPair" /> for every <see cref="Artist" /> in <see cref="Album.Artists" /></returns>
    public ImmutableArray<AlbumArtistPair> GetAlbumArtistPairings( ) {
        return Artists.Select( x => new AlbumArtistPair( Id , x.Id ) ).ToImmutableArray( );
    }
}
