using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210618)]
    // ReSharper disable once UnusedType.Global
    public class Fm20210618AddPlaylistTable : AutoReversingMigration
    {
        private const string Playlist = "Playlist";
        private const string Artist = "Artist";
        private const string Album = "Album";

        /// <summary>
        /// Tracks contained in a Playlist
        /// </summary>
        private const string PlaylistTrack = "PlaylistTrack";

        /// <summary>
        /// Album/Artist many-to-many relationship
        /// </summary>
        private const string AlbumArtist = "AlbumArtist";

        /// <summary>
        /// PlaylistTrack/Artist many-to-many relationship
        /// </summary>
        private const string TrackArtist = "TrackArtist";

        public override void Up()
        {
            CreateTables();
            CreateIndexes();
            CreatePrimaryKeys();
            CreateForeignKeys();
        }

        private void CreateTables()
        {
            Create.Table(Playlist)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("snapshot_id").AsString().Unique().NotNullable()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("collaborative").AsBoolean().NotNullable()
                .WithColumn("description").AsString().Nullable()
                .WithColumn("public").AsBoolean().Nullable();

            Create.Table(Artist)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("url").AsString().NotNullable();

            Create.Table(Album)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("total_tracks").AsInt16().NotNullable()
                .WithColumn("album_type").AsString()
                .WithColumn("release_date").AsDateTime()
                .WithColumn("url").AsString().NotNullable();

            Create.Table(PlaylistTrack)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("track_number").AsInt16().NotNullable()
                .WithColumn("disc_number").AsInt16().NotNullable()
                .WithColumn("added_at").AsDateTime().NotNullable()
                .WithColumn("duration_ms").AsInt64().NotNullable()
                .WithColumn("album_id").AsString().NotNullable()
                .WithColumn("playlist_id").AsString().NotNullable()
                .WithColumn("playlist_snapshot_id").AsString().NotNullable();

            Create.Table(AlbumArtist)
                .WithTimeStamps()
                .WithColumn("album_id").AsString().NotNullable()
                .WithColumn("artist_id").AsString().NotNullable();

            Create.Table(TrackArtist)
                .WithTimeStamps()
                .WithColumn("track_id").AsString().NotNullable()
                .WithColumn("artist_id").AsString().NotNullable();
        }

        private void CreatePrimaryKeys()
        {
            Create.PrimaryKey()
                .OnTable(AlbumArtist)
                .Columns("album_id", "artist_id");

            Create.PrimaryKey()
                .OnTable(TrackArtist)
                .Columns("track_id", "artist_id");
        }

        private void CreateForeignKeys()
        {
            Create.ForeignKey()
                .FromTable(PlaylistTrack).ForeignColumn("album_id")
                .ToTable(Album).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(PlaylistTrack).ForeignColumn("playlist_snapshot_id")
                .ToTable(PlaylistTrack).PrimaryColumn("snapshot_id");

            Create.ForeignKey()
                .FromTable(PlaylistTrack).ForeignColumn("playlist_id")
                .ToTable(PlaylistTrack).PrimaryColumn("id");


            Create.ForeignKey()
                .FromTable(AlbumArtist).ForeignColumn("album_id")
                .ToTable(Album).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(AlbumArtist).ForeignColumn("artist_id")
                .ToTable(Artist).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(TrackArtist).ForeignColumn("track_id")
                .ToTable(PlaylistTrack).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(TrackArtist).ForeignColumn("artist_id")
                .ToTable(Artist).PrimaryColumn("id");
        }

        private void CreateIndexes()
        {
            Create.Index()
                .OnTable(Album)
                .OnColumn("release_date").Ascending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(PlaylistTrack)
                .OnColumn("added_at").Descending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(PlaylistTrack)
                .OnColumn("album_id").Ascending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(PlaylistTrack)
                .OnColumn("playlist_id").Ascending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(PlaylistTrack)
                .OnColumn("playlist_snapshot_id").Ascending()
                .WithOptions().Clustered();
        }
    }
}
