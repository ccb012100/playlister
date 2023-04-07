using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    public record PlaylistTracksRefObject
    {
        public Uri Href { get; init; }
        public int Total { get; init; }
    }
}
