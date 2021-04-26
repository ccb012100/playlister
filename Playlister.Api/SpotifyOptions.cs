using System;
using System.Diagnostics.CodeAnalysis;

namespace Playlister
{
    // ReSharper disable once ClassNeverInstantiated.Global
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class SpotifyOptions
    {
        public const string Spotify = "Spotify";

        public string ClientId { get; } = null!;
        public Uri ApiBaseUrl { get; } = null!;
        public Uri AuthorizationUrl { get; } = null!;
    }
}
