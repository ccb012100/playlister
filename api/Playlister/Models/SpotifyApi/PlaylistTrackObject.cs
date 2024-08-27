#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    public record PlaylistTrackObject
    {
        /// <summary>
        ///     The date and time the track or episode was added. Note that some very old playlists may return <c>null</c> in this field.
        /// </summary>
        public DateTime? AddedAt { get; init; }

        /// <summary>
        ///     The Spotify user who added the track or episode. Note that some very old playlists may return <c>null</c> in this field.
        /// </summary>
        public PublicUserObject? AddedBy { get; init; }

        /// <summary>
        ///     Whether this track or episode is a local file or not.
        /// </summary>
        public bool IsLocal { get; init; }

        /// <summary>
        ///     Information about the track or episode.
        /// </summary>
        public TrackObject Track { get; init; }
    }
}
