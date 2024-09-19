using Playlister.Models.SpotifyApi.Enums;

namespace Playlister.Models.SpotifyApi;

/// <summary>
///     Returned from the <i>Get Artist's Albums</i> endpoint <c>/artists/{id}/albums</c>
/// </summary>
/// <remarks>
///     See <a href="https://developer.spotify.com/documentation/web-api/reference/get-an-artists-albums" />
/// </remarks>
public record SimplifiedAlbumObject : PlaylistItemTrackAlbumObject {
    /// <summary>
    ///     This field represents the relationship between the artist and the album.<br /><br />
    ///     Possible values are <c>album</c>, <c>single</c>, <c>compilation</c>, <c>appears_on</c>.
    /// </summary>
    /// <remarks>
    ///     The field is present when getting an artist’s albums.
    /// </remarks>
    public AlbumGroup AlbumGroup { get; init; }
}
