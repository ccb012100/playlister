namespace Playlister.Models.SpotifyApi;

/// <summary>
///     An Album saved in a Spotify user's 'Your Music' library
/// </summary>
public record SavedAlbumObject : PlaylistItemTrackAlbumObject
{
    /// <summary>
    ///     The date and time the album was saved Timestamps are returned in <b>ISO 8601</b> format
    ///     as Coordinated Universal Time (UTC) with a zero offset: <c>YYYY-MM-DDTHH:MM:SSZ</c>.
    /// </summary>
    public required string AddedAt { get; init; }

    /// <summary>
    ///     Known external IDs for the album.
    /// </summary>
    public required ExternalIdObject ExternalIds { get; init; }

    /// <summary>
    ///     The label associated with the album.
    /// </summary>
    public required string Label { get; init; }
}
