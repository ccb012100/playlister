using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210618)]
    public class Fm20210618AddPlaylistTable : AutoReversingMigration
    {
        public override void Up()
        {
            CreateTables();
            CreateIndexes();
        }

        /*
         * Sqlite doesn't support most ALTER commands, so PK/FK constraints have to be part of table creation
         * (it will just silently not create them).
         */
        private void CreateTables()
        {
            Create.Table(DataTables.Playlist)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("snapshot_id").AsString().Nullable().Unique()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("collaborative").AsBoolean().NotNullable()
                .WithColumn("description").AsString().Nullable()
                .WithColumn("public").AsBoolean().Nullable();

            Create.Table(DataTables.Artist)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("name").AsString().NotNullable();

            Create.Table(DataTables.Album)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("total_tracks").AsInt16().NotNullable()
                .WithColumn("album_type").AsString().Nullable()
                .WithColumn("release_date").AsString().Nullable();

            Create.Table(DataTables.Track)
                .WithSpotifyIdColumn()
                .WithTimeStamps()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("track_number").AsInt16().NotNullable()
                .WithColumn("disc_number").AsInt16().NotNullable()
                .WithColumn("duration_ms").AsInt64().NotNullable()
                .WithColumn("album_id").AsString().NotNullable()
                .ForeignKey("fk_track_albumid", DataTables.Album, "id");

            // PK is (track_id, playlist_id)
            Create.Table(DataTables.PlaylistTrack)
                .WithTimeStamps()
                .WithColumn("track_id").AsString().NotNullable().PrimaryKey()
                .ForeignKey("fk_playlisttrack_trackid", DataTables.Track, "id")
                .WithColumn("playlist_id").AsString().NotNullable().PrimaryKey()
                .ForeignKey("fk_playlisttrack_playlistid", DataTables.Playlist, "id")
                .WithColumn("playlist_snapshot_id").AsString().Nullable()
                .WithColumn("added_at").AsDateTime().NotNullable();

            // PK is (album_id, artist_id)
            Create.Table(DataTables.AlbumArtist)
                .WithTimeStamps()
                .WithColumn("album_id").AsString().NotNullable().PrimaryKey()
                .ForeignKey("fk_albumartist_albumid", DataTables.Album, "id")
                .WithColumn("artist_id").AsString().NotNullable().PrimaryKey()
                .ForeignKey("fk_albumartist_artistid", DataTables.Artist, "id");

            // PK is (track_id, artist_id)
            Create.Table(DataTables.TrackArtist)
                .WithTimeStamps()
                .WithColumn("track_id").AsString().NotNullable().PrimaryKey()
                .ForeignKey("fk_trackartist_trackid", DataTables.Track, "id")
                .WithColumn("artist_id").AsString().NotNullable().PrimaryKey()
                .ForeignKey("fk_trackartist_artistid", DataTables.Artist, "id");
        }

        private void CreateIndexes()
        {
            Create.Index()
                .OnTable(DataTables.Album)
                .OnColumn("release_date").Ascending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(DataTables.PlaylistTrack)
                .OnColumn("added_at").Descending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(DataTables.PlaylistTrack)
                .OnColumn("album_id").Ascending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(DataTables.PlaylistTrack)
                .OnColumn("playlist_id").Ascending()
                .WithOptions().Clustered();

            Create.Index()
                .OnTable(DataTables.PlaylistTrack)
                .OnColumn("playlist_snapshot_id").Ascending()
                .WithOptions().Clustered();
        }
    }
}
