namespace Playlister.Models.SpotifyApi;

public record ExternalUrlObject
{
    /// <summary>
    ///     The Spotify URL for the object.
    /// </summary>
    public required Uri Spotify { get; init; }
}
