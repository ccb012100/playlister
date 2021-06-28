using System;

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
        // ReSharper disable once MemberCanBePrivate.Global
        public DateTime AddedAt { get; init; }
        public Track Track { get; init; }

        public PlaylistTrack ToPlaylistTrack(MinimalPlaylist playlist) => new()
        {
            Id = Track.Id,
            Name = Track.Name,
            TrackNumber = Track.TrackNumber,
            DiscNumber = Track.DiscNumber,
            AddedAt = AddedAt,
            DurationMs = Track.DurationMs,
            AlbumId = Track.Album.Id,
            PlaylistId = playlist.Id,
            SnapshotId = playlist.SnapshotId
        };
    }
}
