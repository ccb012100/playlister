using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
#pragma warning disable 8618

namespace Playlister.Models
{
    public record Track
    {
        public Album Album { get; init; }
        public ICollection<Artist> Artists { get; init; }
        public int DiscNumber { get; init; }
        public int DurationMs { get; init; }
        public string Id { get; init; }
        public string Name { get; init; }
        public int TrackNumber { get; init; }
    }
}
