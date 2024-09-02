namespace Playlister.Models.SpotifyApi;

/// <summary>
///     This doesn't exist on the Spotify documentation, but seems to be a thing.
/// </summary>
public record SimplifiedUserObject
{
    public required ExternalUrlObject ExternalUrls { get; init; }
    public required Uri Href { get; init; }
    public required string Id { get; init; }

    /// <summary>
    ///     Always "user"
    /// </summary>
    public required string Type { get; init; }

    public required string Uri { get; init; }
}
