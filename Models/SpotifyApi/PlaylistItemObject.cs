using System;

// ReSharper disable UnusedMember.Global
#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    /// <summary>
    /// This isn't documented, but it's what the "Get a Playlist's Items" API method actually returns.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public record PlaylistItemObject
    {
        public DateTime AddedAt { get; init; }
        public SimplifiedUserObject AddedBy { get; set; }
        public SimplifiedTrackObject Track { get; init; }
        public ThumbnailObject VideoThumbnail { get; init; }
    }
}
