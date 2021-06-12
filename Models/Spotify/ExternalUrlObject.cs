using System;

namespace Playlister.Models.Spotify
{
    public record ExternalUrlObject
    {
        /// <summary>
        /// The Spotify URL for the object.
        /// </summary>
        public Uri Spotify { get; init; }
    }
}
