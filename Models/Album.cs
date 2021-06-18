using System.Collections.Generic;
using Playlister.Models.SpotifyApi.Enums;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

namespace Playlister.Models
{
    public record Album
    {
        public AlbumType AlbumType { get; init; }
        public ICollection<Artist> Artists { get; init; }
        public string Id { get; init; }
        public string Name { get; init; }
        public string ReleaseDate { get; set; }
        public ReleaseDatePrecision ReleaseDatePrecision { get; set; }
        public int TotalTracks { get; init; }
    }
}
