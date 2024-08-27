#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    public record PrivateUserObject : ISpotifyApiObject
    {
        /// <summary>
        ///     The country of the user, as set in the user’s account profile.
        ///     An <c>ISO 3166-1 alpha-2</c> country code.
        ///     This field is only available when the current user has granted access to the user-read-private scope.
        /// </summary>
        public string Country { get; init; }

        /// <summary>
        ///     The name displayed on the user’s profile. <c>null</c> if not available.
        /// </summary>
        public string? DisplayName { get; init; }

        /// <summary>
        ///     The user’s email address, as entered by the user when creating their account.
        ///     Important! This email address is unverified; there is no proof that it actually belongs to the user.
        ///     This field is only available when the current user has granted access to the user-read-email scope.
        /// </summary>
        public string Email { get; init; }

        /// <summary>
        ///     Known public external URLs for this user.
        /// </summary>
        public ExternalUrlObject ExternalUrls { get; init; }

        /// <summary>
        ///     Information about the followers of this user.
        /// </summary>
        public FollowersObject Followers { get; init; }

        /// <summary>
        ///     The user’s profile image.
        /// </summary>
        public IEnumerable<ImageObject> Images { get; init; }

        /// <summary>
        ///     The user’s Spotify subscription level: “premium”, “free”, etc.
        ///     (The subscription level “open” can be considered the same as “free”.)
        ///     This field is only available when the current user has granted access to the user-read-private scope.
        /// </summary>
        public string? Product { get; init; }

        /// <summary>
        ///     A link to the Web API endpoint for this user.
        /// </summary>
        public Uri Href { get; init; }

        /// <summary>
        ///     The Spotify user ID for this user.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        ///     The object type: <c>user</c>.
        /// </summary>
        public string Type { get; init; }

        /// <summary>
        ///     The Spotify URI for this user.
        /// </summary>
        public Uri Uri { get; init; }
    }
}
