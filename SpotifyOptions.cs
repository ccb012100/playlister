using System;
using System.Diagnostics.CodeAnalysis;

namespace Playlister
{
    // ReSharper disable once ClassNeverInstantiated.Global
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class SpotifyOptions
    {
        public const string Spotify = "Spotify";

        public string ClientId { get; }
        public Uri ApiBaseUrl { get; }
        public Uri AuthorizationUrl { get; }
    }
}
