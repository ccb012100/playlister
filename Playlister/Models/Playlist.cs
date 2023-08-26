
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

        /// <summary>
        /// Number of tracks in the playlist
        /// </summary>
        public int Count { get; init; }

        public override string ToString() => $"Playlist `{Id}` (\"{Name}\")";
    }
}
