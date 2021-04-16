using System;

namespace Playlister
{
    public class SpotifyOptions
    {
        public const string Spotify = "Spotify";

        public string ClientId { get; init; }
        public Uri ApiBaseUrl { get; init; }
    }
}
