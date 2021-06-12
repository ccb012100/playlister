using System;

namespace Playlister.Models.Spotify
{
    public record PlaylistObject
    {
        private readonly string _type;

        /// <summary>
        /// `true` if the owner allows other users to modify the playlist.
        /// </summary>
        public bool Collaborative { get; init; }

        /// <summary>
        /// The playlist description. Only returned for modified, verified playlists, otherwise `null`.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Known external URLs for this playlist.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

        /// <summary>
        /// Information about the followers of the playlist.
        /// </summary>
        public FollowersObject Followers { get; init; }

        /// <summary>
        /// A link to the Web API endpoint providing full details of the playlist.
        /// </summary>
        public Uri Href { get; init; }

        /// <summary>
        /// The Spotify ID for the playlist.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Images for the playlist. The array may be empty or contain up to three images.
        /// The images are returned by size in descending order.
        /// Note: If returned, the source URL for the image (url) is temporary and will expire in less than a day.
        /// </summary>
        public ImageObject[] Images { get; init; }

        /// <summary>
        /// The name of the playlist.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The user who owns the playlist
        /// </summary>
        public PublicUserObject Owner { get; init; }

        /// <summary>
        /// The playlist’s public/private status:
        /// `true` the playlist is public,
        /// `false` the playlist is private,
        /// `null` the playlist status is not relevant.
        /// </summary>
        public bool? Public { get; init; }

        /// <summary>
        /// The version identifier for the current playlist. Can be supplied in other requests to target a specific playlist version.
        /// </summary>
        public string SnapshotId { get; init; }

        /// <summary>
        /// Information about the tracks of the playlist. Note, a track object may be `null`. This can happen if a track is no longer available.
        /// </summary>
        public PlaylistTrackObject?[] Tracks { get; init; }

        /// <summary>
        /// The object type: “playlist”.
        /// </summary>
        public string Type
        {
            get => _type;
            init
            {
                if (value != "playlist")
                {
                    throw new ArgumentException($"Invalid value `{value}`. Expected `playlist`");
                }

                _type = value;
            }
        }

        /// <summary>
        /// The Spotify URI for the playlist.
        /// </summary>
        public Uri Uri { get; init; }
    }
}
