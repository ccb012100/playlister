using System.Text.Json.Serialization;
using Playlister.Models.SpotifyApi.Enums;

namespace Playlister.Models.SpotifyApi;

public record SimplifiedPlaylistObject : ISpotifyApiObject
{
    /// <summary>
    ///     <c>true</c> if the owner allows other users to modify the playlist.
    /// </summary>
    public bool Collaborative { get; init; }

    /// <summary>
    ///     The playlist description. Only returned for modified, verified playlists, otherwise <c>null</c>.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Known external URLs for this playlist.
    /// </summary>
    public ExternalUrlObject? ExternalUrls { get; init; }

    /// <summary>
    ///     Images for the playlist. The array may be empty or contain up to three images.
    ///     The images are returned by size in descending order.
    ///     Note: If returned, the source URL for the image (url) is temporary and will expire in less than a day.
    /// </summary>
    public required ICollection<ImageObject> Images { get; init; }

    /// <summary>
    ///     The name of the playlist.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The user who owns the playlist
    /// </summary>
    public required PublicUserObject Owner { get; init; }

    /// <summary>
    ///     The playlist’s public/private status:<br /><br />
    ///     <c>true</c> = the playlist is public,<br />
    ///     <c>false</c> = the playlist is private,<br />
    ///     <c>null</c> = the playlist status is not relevant.
    /// </summary>
    public bool? Public { get; init; }

    /// <summary>
    ///     The version identifier for the current playlist. Can be supplied in other requests to target a specific playlist version.
    /// </summary>
    [JsonPropertyName( "snapshot_id" )]
    public string? SnapshotId { get; init; }

    /// <summary>
    ///     A collection containing a link <c>href</c> to the Web API endpoint where full details of the playlist’s tracks can be retrieved, along with the
    ///     total number of tracks in the playlist.
    ///     Note, a track object may be null. This can happen if a track is no longer available.
    /// </summary>
    public PlaylistTracksRefObject Tracks { get; init; } = null!;

    /// <summary>
    ///     A link to the Web API endpoint providing full details of the playlist.
    /// </summary>
    public required Uri Href { get; init; }

    /// <summary>
    ///     The Spotify ID for the playlist.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     The object type: <c>playlist</c>
    /// </summary>
    public required ObjectType Type { get; init; }

    /// <summary>
    ///     The Spotify URI for the playlist.
    /// </summary>
    public required Uri Uri { get; init; }

    public Playlist ToPlaylist()
    {
        return new Playlist
        {
            Id = Id,
            SnapshotId = SnapshotId,
            Name = Name,
            Collaborative = Collaborative,
            Description = Description,
            Public = Public,
            Count = Tracks.Total
        };
    }
}
