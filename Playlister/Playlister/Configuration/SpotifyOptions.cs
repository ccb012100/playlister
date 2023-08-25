using System;

#pragma warning disable 8618

namespace Playlister.Configuration
{
    public record SpotifyOptions
    {
        public const string Spotify = "Spotify";

        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public Uri ApiBaseAddress { get; init; }
        public Uri AccountsApiBaseAddress { get; init; }
        public Uri CallbackUrl { get; init; }
    }
}
