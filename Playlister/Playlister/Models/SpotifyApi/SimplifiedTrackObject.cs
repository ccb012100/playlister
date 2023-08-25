using System;
using System.Collections.Generic;

#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    public record SimplifiedTrackObject : ISpotifyApiObject
    {
        /// <summary>
        /// The artists who performed the track. Each artist object includes a link in <c>href</c> to more detailed information about the artist.
        /// </summary>
        public IEnumerable<SimplifiedArtistObject> Artists { get; init; }

        /// <summary>
        /// A list of the countries in which the track can be played, identified by their <c>ISO 3166-1 alpha-2</c> code
        /// </summary>
        public IEnumerable<string> AvailableMarkets { get; init; }

        /// <summary>
        /// The disc number (usually <c>1</c> unless the album consists of more than one disc).
        /// </summary>
        public int DiscNumber { get; init; }

        /// <summary>
        /// The track length in milliseconds.
        /// </summary>
        public int DurationMs { get; init; }

        /// <summary>
        /// Whether or not the track has explicit lyrics (<c>true</c> = yes it does; <c>false</c> = no it does not OR unknown).
        /// </summary>
        public bool Explicit { get; init; }

        /// <summary>
        /// Known external URLs for this track.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

        /// <summary>
        /// A link to the Web API endpoint providing full details of the track.
        /// </summary>
        public Uri Href { get; init; }

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
        /// If <c>true</c>, the track is playable in the given market. Otherwise <c>false</c>.
        /// </summary>
        public bool? IsPlayable { get; init; }

        /// <summary>
        /// Part of the response when Track Relinking is applied, and the requested track has been replaced with different track.
        /// The track in the <c>linked_from</c> object contains information about the originally requested track.
        /// </summary>
        public string? LinkedFrom { get; init; }

        /// <summary>
        /// The name of the track.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// A link to a 30 second preview (MP3 format) of the track. Can be <c>null</c>
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
        /// The object type: <c>track</c>.
        /// </summary>
        public string Type { get; init; }

        /// <summary>
        /// The Spotify URI for the track.
        /// </summary>
        public Uri Uri { get; init; }
    }
}
