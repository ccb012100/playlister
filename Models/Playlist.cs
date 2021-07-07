// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;

#pragma warning disable 8618
namespace Playlister.Models
{
    public record Playlist
    {
        public string Id { get; init; }

        [JsonPropertyName("snapshot_id")]
        public string? SnapshotId { get; init; }

        public string Name { get; init; }

        public bool Collaborative { get; init; }

        public string? Description { get; init; }

        public bool? Public { get; init; }

        public override string ToString() => $"Playlist `{Id}` (\"{Name}\")";
    }
}
