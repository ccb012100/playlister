using System.Text.Json.Serialization;

#pragma warning disable 8618

namespace Playlister.Models
{
    /// <summary>
    ///     Playlist Item
    /// </summary>
    public record PlaylistItem
    {
        [JsonPropertyName("added_at")] public DateTime AddedAt { get; init; }

        public Track Track { get; init; }

        public PlaylistTrack ToPlaylistTrack(Playlist playlist) => new()
        {
            TrackId = Track.Id,
            AddedAt = AddedAt,
            PlaylistId = playlist.Id,
            SnapshotId = playlist.SnapshotId
        };
    }
}
