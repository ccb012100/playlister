namespace Playlister.Models.SpotifyApi;

public record FollowersObject {
    /// <summary>
    ///     A link to the Web API endpoint providing full details of the followers; <c>null</c> if not available.
    ///     Please note that this will always be set to null, as the Web API does not support it at the moment.
    /// </summary>
    public string? Href { get; init; }

    /// <summary>
    ///     The total number of followers.
    /// </summary>
    public int Total { get; init; }
}
