namespace Playlister.Models.SpotifyApi;

public record SimplifiedArtistObject
{
    /// <summary>
    ///     Known external URLs for this artist.
    /// </summary>
    public required ExternalUrlObject ExternalUrls { get; init; }

    /// <summary>
    ///     A link to the Web API endpoint providing full details of the artist.
    /// </summary>
    public required string Href { get; init; }

    /// <summary>
    ///     The name of the artist.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The object type: <c>artist</c>
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    ///     The Spotify URI for the artist.
    /// </summary>
    public required Uri Uri { get; init; }
}
