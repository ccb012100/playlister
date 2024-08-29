#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi;

/// <summary>
///     This doesn't exist on the Spotify documentation, but seems to be a thing.
/// </summary>
public record SimplifiedUserObject
{
    public ExternalUrlObject ExternalUrls { get; init; }
    public Uri Href { get; init; }
    public string Id { get; init; }

    /// <summary>
    ///     Always "user"
    /// </summary>
    public string Type { get; init; }

    public string Uri { get; init; }
}
