namespace Playlister.Data
{
    public static class DataTables
    {
        public const string Playlist = "Playlist";
        public const string Artist = "Artist";
        public const string Album = "Album";
        public const string AccessToken = "AccessToken";

        /// <summary>
        /// Tracks contained in a Playlist
        /// </summary>
        public const string PlaylistTrack = "PlaylistTrack";

        /// <summary>
        /// Album/Artist many-to-many relationship
        /// </summary>
        public const string AlbumArtist = "AlbumArtist";

        /// <summary>
        /// PlaylistTrack/Artist many-to-many relationship
        /// </summary>
        public const string TrackArtist = "TrackArtist";
    }
}
