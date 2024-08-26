using System;

#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    public record ExternalUrlObject
    {
        /// <summary>
        ///     The Spotify URL for the object.
        /// </summary>
        public Uri Spotify { get; init; }
    }
}
