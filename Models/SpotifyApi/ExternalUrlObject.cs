using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record ExternalUrlObject
    {
        /// <summary>
        /// The Spotify URL for the object.
        /// </summary>
        public Uri Spotify { get; init; }
    }
}
