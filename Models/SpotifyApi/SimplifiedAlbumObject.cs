using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Playlister.Models.Enums;

namespace Playlister.Models.SpotifyApi
{
    public record SimplifiedAlbumObject
    {
        private SpotifyApiObjectType _type;

        /// <summary>
        /// The field is present when getting an artist’s albums.
        /// Possible values are “album”, “single”, “compilation”, “appears_on”.
        /// Compare to `album_type` this field represents relationship between the artist and the album.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public string? AlbumGroup { get; init; }

        /// <summary>
        /// The type of the album: one of “album”, “single”, or “compilation”.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public string AlbumType { get; init; }

        /// <summary>
        /// The artists of the album.
        /// Each artist object includes a link in `href` to more detailed information about the artist.
        /// </summary>
        public ICollection<SimplifiedArtistObject> Artists { get; init; }

        /// <summary>
        /// The markets in which the album is available: `ISO 3166-1 alpha-2` country codes.
        /// Note that an album is considered available in a market when at least 1 of its tracks is available in that market.
        /// </summary>
        public ICollection<string> AvailableMarkets { get; init; }

        /// <summary>
        /// Known external URLs for this album.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

        /// <summary>
        /// A link to the Web API endpoint providing full details of the album.
        /// </summary>
        public Uri Href { get; init; }

        /// <summary>
        /// The Spotify ID for the album.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// The cover art for the album in various sizes, widest first.
        /// </summary>
        public ICollection<ImageObject> Images { get; init; }

        /// <summary>
        /// The name of the album. In case of an album takedown, the value may be an empty string.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The date the album was first released, for example `1981`.
        /// Depending on the precision, it might be shown as `1981-12` or `1981-12-15`.
        /// </summary>
        public string ReleaseDate { get; init; }

        /// <summary>
        /// The precision with which `release_date` value is known: `year` , `month` , or `day`.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ReleaseDatePrecision ReleaseDatePrecision { get; init; }

        /// <summary>
        /// Included in the response when a content restriction is applied.
        /// </summary>
        public AlbumRestrictionObject Restrictions { get; init; }

        /// <summary>
        /// The total number of tracks in the album.
        /// </summary>
        public int TotalTracks { get; init; }

        /// <summary>
        /// The object type: “album"
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SpotifyApiObjectType Type { get; init; }

        /// <summary>
        /// The Spotify URI for the album.
        /// </summary>
        public Uri Uri { get; init; }
    }
}
