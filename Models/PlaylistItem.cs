using System;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable UnusedMember.Global
#pragma warning disable 8618

namespace Playlister.Models
{
    /// <summary>
    /// Playlist Item
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public record PlaylistItem
    {
        [JsonPropertyName("added_at")]
        // ReSharper disable once MemberCanBePrivate.Global
        public DateTime AddedAt { get; init; }

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
