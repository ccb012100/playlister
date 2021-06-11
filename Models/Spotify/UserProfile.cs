// ReSharper disable UnusedType.Global

using System;

#pragma warning disable 8618
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Playlister.Models.Spotify
{
    public record UserProfile
    {
        public string Country { get; init; }
        public string DisplayName { get; init; }
        public string Href { get; init; }
        public string Id { get; init; }
        public Image[] Images { get; init; }
        public string Product { get; init; }
        public string Type { get; init; }
        public string Uri { get; init; }

        public record ExplicitContent
        {
            public bool FilterEnabled { get; init; }
            public bool FilterLocked { get; init; }
        }

        public record ExternalUrls
        {
            public string Spotify { get; init; }
        }

        public record Followers
        {
            public string? Href { get; init; }
            public int Total { get; init; }
        }

        public record Image
        {
            public int? Height { get; init; }
            public Uri Url { get; init; }
            public int? Width { get; init; }
        }
    }
}
