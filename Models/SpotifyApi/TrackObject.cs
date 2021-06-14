using System;
using System.Collections.Generic;

namespace Playlister.Models.SpotifyApi
{
    public record TrackObject
    {
        private readonly string _type;

        /// <summary>
        /// The album on which the track appears. The album object includes a link in `href` to full information about the album.
        /// </summary>
        public SimplifiedAlbumObject Album { get; init; }

        /// <summary>
        /// The artists who performed the track. Each artist object includes a link in `href` to more detailed information about the artist.
        /// </summary>
        public IEnumerable<ArtistObject> Artists { get; init; }

        /// <summary>
        /// A list of the countries in which the track can be played, identified by their `ISO 3166-1 alpha-2` code
        /// </summary>
        public IEnumerable<string> AvailableMarkets { get; init; }

        /// <summary>
        /// The disc number (usually `1` unless the album consists of more than one disc).
        /// </summary>
        public int DiscNumber { get; init; }

        /// <summary>
        /// The track length in milliseconds.
        /// </summary>
        public int DurationMs { get; init; }

        /// <summary>
        /// Whether or not the track has explicit lyrics (`true` = yes it does; `false` = no it does not OR unknown).
        /// </summary>
        public bool Explicit { get; init; }

        /// <summary>
        /// Known external IDs for the track.
        /// </summary>
        public ExternalIdObject ExternalIds { get; init; }

        /// <summary>
        /// Known external URLs for this track.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

        /// <summary>
        /// A link to the Web API endpoint providing full details of the track.
        /// </summary>
        public string Href { get; init; }

        /// <summary>
        /// The Spotify ID for the track.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Whether or not the track is from a local file.
        /// </summary>
        public string IsLocal { get; init; }

        /// <summary>
        /// Part of the response when Track Relinking is applied.
        /// If `true`, the track is playable in the given market. Otherwise `false`.
        /// </summary>
        public bool? IsPlayable { get; init; }

        /// <summary>
        /// Part of the response when Track Relinking is applied, and the requested track has been replaced with different track.
        /// The track in the `linked_from` object contains information about the originally requested track.
        /// </summary>
        public string? LinkedFrom { get; init; }

        /// <summary>
        /// The name of the track.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The popularity of the track. The value will be between 0 and 100, with 100 being the most popular.
        /// The popularity is calculated by algorithm and is based, in the most part, on the total number of plays the track has had and how recent those plays are.
        /// Generally speaking, songs that are being played a lot now will have a higher popularity than songs that were played a lot in the past.
        /// Duplicate tracks (e.g. the same track from a single and an album) are rated independently.
        /// Artist and album popularity is derived mathematically from track popularity.
        /// Note that the popularity value may lag actual popularity by a few days: the value is not updated in real time.
        /// </summary>
        public int Popularity { get; init; }

        /// <summary>
        /// A link to a 30 second preview (MP3 format) of the track. Can be `null`
        /// </summary>
        public Uri? PreviewUrl { get; init; }

        /// <summary>
        /// Included in the response when a content restriction is applied.
        /// </summary>
        public TrackRestrictionObject? Restrictions { get; init; }

        /// <summary>
        /// The number of the track. If an album has several discs, the track number is the number on the specified disc.
        /// </summary>
        public int TrackNumber { get; init; }

        /// <summary>
        /// The object type: “track”.
        /// </summary>
        public string Type
        {
            get => _type;
            init
            {
                if (value != "track")
                {
                    throw new ArgumentException($"Value `{value}` was not expected value `track`.");
                }

                _type = value;
            }
        }

        /// <summary>
        /// The Spotify URI for the track.
        /// </summary>
        public Uri Uri { get; init; }
    }
}
