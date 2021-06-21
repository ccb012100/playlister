using System;
using System.Collections.Generic;
using Playlister.Models.SpotifyApi.Enums;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global
#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    public record SimplifiedPlaylistObject : ISpotifyApiObject
    {
        /// <summary>
        /// <c>true</c> if the owner allows other users to modify the playlist.
        /// </summary>
        public bool Collaborative { get; init; }

        /// <summary>
        /// The playlist description. Only returned for modified, verified playlists, otherwise <c>null</c>.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Known external URLs for this playlist.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

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
        public ICollection<ImageObject> Images { get; init; }

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
        /// <c>true</c> the playlist is public,
        /// <c>false</c> the playlist is private,
        /// <c>null</c> the playlist status is not relevant.
        /// </summary>
        public bool? Public { get; init; }

        /// <summary>
        /// The version identifier for the current playlist. Can be supplied in other requests to target a specific playlist version.
        /// </summary>
        public string? SnapshotId { get; init; }

        /// <summary>
        /// A collection containing a link <c>href</c> to the Web API endpoint where full details of the playlist’s tracks can be retrieved, along with the total number of tracks in the playlist.
        /// Note, a track object may be null. This can happen if a track is no longer available.
        /// </summary>
        public PlaylistTracksRefObject Tracks { get; init; }

        /// <summary>
        /// The object type: <c>playlist</c>
        /// </summary>
        public string Type { get; init; }

        /// <summary>
        /// The Spotify URI for the playlist.
        /// </summary>
        public Uri Uri { get; init; }

        public Playlist ToPlaylist()
        {
            return new()
            {
                Id = Id,
                SnapshotId = SnapshotId,
                Name = Name,
                Collaborative = Collaborative,
                Description = Description,
                Public = Public
            };
        }
    }
}
