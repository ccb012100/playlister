// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618
namespace Playlister.Models
{
    public record Playlist
    {
        public string Id { get; init; }
        public string? SnapshotId { get; init; }
        public string Name { get; init; }
        public bool Collaborative { get; init; }
        public string? Description { get; init; }
        public bool? Public { get; init; }
    }
}
