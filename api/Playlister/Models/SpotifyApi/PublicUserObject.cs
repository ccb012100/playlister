namespace Playlister.Models.SpotifyApi;

public record PublicUserObject {
    /// <summary>
    ///     The name displayed on the user’s profile. <c>null</c> if not available.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    ///     Known public external URLs for this user.
    /// </summary>
    public ExternalUrlObject? ExternalUrls { get; init; }

    /// <summary>
    ///     Information about the followers of this user.
    /// </summary>
    public FollowersObject? Followers { get; init; }

    /// <summary>
    ///     A link to the Web API endpoint for this user.
    /// </summary>
    public required Uri Href { get; init; }

    /// <summary>
    ///     The Spotify user ID for this user.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     The user’s profile image.
    /// </summary>
    public IEnumerable<ImageObject>? Images { get; init; }

    /// <summary>
    ///     The object type: <c>user</c>.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    ///     The Spotify URI for this user.
    /// </summary>
    public required Uri Uri { get; set; }
}
