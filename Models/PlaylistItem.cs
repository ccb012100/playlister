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
        public DateTime AddedAt { get; init; }
        public Track Track { get; init; }
    }
}