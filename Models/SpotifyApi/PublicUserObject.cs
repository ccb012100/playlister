using System;
using System.Collections.Generic;

namespace Playlister.Models.SpotifyApi
{
    public record PublicUserObject
    {
        private readonly string _type;

        /// <summary>
        /// The name displayed on the user’s profile. `null` if not available.
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

        // TODO: set validation on value
        /// <summary>
        /// The object type: “user”.
        /// </summary>
        public string Type
        {
            get => _type;
            init
            {
                if (value != "user")
                {
                    throw new ArgumentException($"Value `{value}` does not match expected value `user`.");
                }

                _type = value;
            }
        }

        /// <summary>
        /// The Spotify URI for this user.
        /// </summary>
        public Uri Uri { get; set; }
    }
}
