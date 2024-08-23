using System;

namespace Playlister.Models.SpotifyApi
{
    public interface ISpotifyApiObject
    {
        /// <summary>
        ///     A link to the Web API endpoint providing full details of the object.
        /// </summary>
        public Uri Href { get; init; }

        /// <summary>
        ///     The Spotify ID for the Object.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        ///     The object type
        /// </summary>
        public string Type { get; init; }

        /// <summary>
        ///     The Spotify URI for the Object.
        /// </summary>
        public Uri Uri { get; init; }
    }
}
