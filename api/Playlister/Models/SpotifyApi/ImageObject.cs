#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi;

public record ImageObject
{
    /// <summary>
    ///     The image height in pixels. If unknown: <c>null</c> or not returned.
    /// </summary>
    public int? Height { get; init; }

    /// <summary>
    ///     The source URL of the image.
    /// </summary>
    public Uri Url { get; init; }

    /// <summary>
    ///     The image width in pixels. If unknown: <c>null</c> or not returned.
    /// </summary>
    public int? Width { get; init; }
}
