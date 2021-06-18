using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record PlaylistTracksRefObject
    {
        public Uri Href { get; init; }
        public int Total { get; init; }
    }
}
