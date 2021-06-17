using System;
using System.Collections.Generic;
using Playlister.Models.Enums;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    public record PublicUserObject
    {
        /// <summary>
        /// The name displayed on the user’s profile. <c>null</c> if not available.
        /// </summary>
        public string? DisplayName { get; init; }

        /// <summary>
        /// Known public external URLs for this user.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

        /// <summary>
        /// Information about the followers of this user.
        /// </summary>
        public FollowersObject Followers { get; init; }

        /// <summary>
        /// A link to the Web API endpoint for this user.
        /// </summary>
        public Uri Href { get; init; }

        /// <summary>
        /// The Spotify user ID for this user.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// The user’s profile image.
        /// </summary>
        public IEnumerable<ImageObject> Images { get; init; }

        /// <summary>
        /// The object type: <c>user</c>.
        /// </summary>
        public SpotifyApiObjectType Type { get; init; }

        /// <summary>
        /// The Spotify URI for this user.
        /// </summary>
        public Uri Uri { get; set; }
    }
}
