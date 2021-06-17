using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public record PlaylistObject : SimplifiedPlaylistObject
    {
        /// <summary>
        /// Information about the followers of the playlist.
        /// </summary>
        public FollowersObject Followers { get; init; }

        /// <summary>
        /// Information about the tracks of the playlist.
        /// Note, a track object may be <c>null</c>. This can happen if a track is no longer available.
        /// </summary>
        public new ICollection<PlaylistTrackObject?> Tracks { get; init; }
    }
}
