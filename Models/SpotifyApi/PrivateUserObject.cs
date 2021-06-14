using System;
using System.Collections.Generic;

namespace Playlister.Models.SpotifyApi
{
    public record PrivateUserObject
    {
        private readonly string _type = null!;

        /// <summary>
        /// The country of the user, as set in the user’s account profile.
        /// An ISO 3166-1 alpha-2 country code.
        /// This field is only available when the current user has granted access to the user-read-private scope.
        /// </summary>
        public string Country { get; init; }

        /// <summary>
        /// The name displayed on the user’s profile. `null` if not available.
        /// </summary>
        public string? DisplayName { get; init; }

        /// <summary>
        /// The user’s email address, as entered by the user when creating their account.
        /// Important! This email address is unverified; there is no proof that it actually belongs to the user.
        /// This field is only available when the current user has granted access to the user-read-email scope.
        /// </summary>
        public string Email { get; init; }

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
        /// The user’s Spotify subscription level: “premium”, “free”, etc.
        /// (The subscription level “open” can be considered the same as “free”.)
        /// This field is only available when the current user has granted access to the user-read-private scope.
        /// </summary>
        public string? Product { get; init; }

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
                    throw new ArgumentException($"Invalid value `{value}`. Expected `user`.");
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
