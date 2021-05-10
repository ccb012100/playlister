using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618
namespace Playlister
{
    // ReSharper disable once ClassNeverInstantiated.Global
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class SpotifyOptions
    {
        public const string Spotify = "Spotify";

        public string ClientId { get; init; }
        public Uri ApiBaseAddress { get; init; }
        public Uri AccountsApiBaseAddress { get; init; }
        public Uri CallbackUrl { get; init; }
    }
}