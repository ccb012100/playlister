using System.Text.Json.Serialization;

namespace Playlister.Models;

/// <summary>
///     Playlist Item
/// </summary>
public record PlaylistItem
{
    [JsonPropertyName( "added_at" )] public DateTime AddedAt { get; init; }

    public required Track Track { get; init; }

    public PlaylistTrack ToPlaylistTrack( Playlist playlist )
    {
        return new PlaylistTrack
        {
            TrackId = Track.Id,
            AddedAt = AddedAt,
            PlaylistId = playlist.Id,
            SnapshotId = playlist.SnapshotId
        };
    }
}
