using System;
using Playlister.Attributes;
using Playlister.Models.Enums;

namespace Playlister.Models.SpotifyApi
{
    public record SimplifiedArtistObject
    {
        /// <summary>
        /// Known external URLs for this artist.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

        /// <summary>
        /// A link to the Web API endpoint providing full details of the artist.
        /// </summary>
        public string Href { get; init; }

        /// <summary>
        ///  The name of the artist.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The object type: <c>artist</c>
        /// </summary>
        [ValidateSpotifyApiObjectType(SpotifyApiObjectType.Artist)]
        public SpotifyApiObjectType Type { get; init; }

        /// <summary>
        /// The Spotify URI for the artist.
        /// </summary>
        public Uri Uri { get; init; }
    }
}
