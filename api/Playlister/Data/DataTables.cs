namespace Playlister.Data;

public static class DataTables
{
    [Obsolete( "Not used anymore" )] public const string AccessToken = "AccessToken";

    public const string Album = "Album";
    public const string Artist = "Artist";
    public const string Playlist = "Playlist";
    public const string Track = "Track";

    /// <summary>
    ///     Instances of a Track in a Playlist
    /// </summary>
    public const string PlaylistTrack = "PlaylistTrack";

    /// <summary>
    ///     Album/Artist many-to-many relationship
    /// </summary>
    public const string AlbumArtist = "AlbumArtist";

    /// <summary>
    ///     Track/Artist many-to-many relationship
    /// </summary>
    public const string TrackArtist = "TrackArtist";

    /// <summary>
    ///     External IDs for a Saved Album
    /// </summary>
    public const string ExternalId = "ExternalId";

    /// <summary>
    ///     Albums saved in a Spotify user's 'Your Music' library
    /// </summary>
    public const string SavedAlbum = "SavedAlbum";
}
