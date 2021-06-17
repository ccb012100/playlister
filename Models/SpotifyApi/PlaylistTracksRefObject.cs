using System;

namespace Playlister.Models.SpotifyApi
{
    public record PlaylistTracksRefObject
    {
        public Uri Href { get; init; }
        public int Total { get; init; }
    }
}