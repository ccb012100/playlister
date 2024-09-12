namespace Playlister.Data;

public static class DataTables
{
    [Obsolete( "Not used anymore" )] public const string AccessToken = "AccessToken";

    public const string Album = nameof( Album );
    public const string Artist = nameof( Artist );
    public const string Playlist = nameof( Playlist );
    public const string Track = nameof( Track );

    /// <summary>
    ///     Instances of a Track in a Playlist
    /// </summary>
    public const string PlaylistTrack = nameof( PlaylistTrack );

    /// <summary>
    ///     Album/Artist many-to-many relationship
    /// </summary>
    public const string AlbumArtist = nameof( AlbumArtist );

    /// <summary>
    ///     Track/Artist many-to-many relationship
    /// </summary>
    public const string TrackArtist = nameof( TrackArtist );

    /// <summary>
    ///     External IDs for a Saved Album
    /// </summary>
    public const string ExternalId = nameof( ExternalId );

    /// <summary>
    ///     Albums saved in a Spotify user's 'Your Music' library
    /// </summary>
    public const string SavedAlbum = nameof( SavedAlbum );

    /// <summary>
    /// Used for searching for Albums
    /// </summary>
    public const string PlaylistAlbum = nameof( PlaylistAlbum );
}
