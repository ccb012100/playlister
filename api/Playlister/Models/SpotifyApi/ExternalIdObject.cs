namespace Playlister.Models.SpotifyApi;

public record ExternalIdObject
{
    /// <summary>
    ///     International Article Number
    /// </summary>
    public required string Ean { get; init; }

    /// <summary>
    ///     International Standard Recording Code
    /// </summary>
    public required string Isrc { get; init; }

    /// <summary>
    ///     Universal Product Code
    /// </summary>
    public required string Upc { get; init; }
}
