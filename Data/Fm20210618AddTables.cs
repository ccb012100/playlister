using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210618)]
    // ReSharper disable once UnusedType.Global
    public class Fm20210618AddPlaylistTable : AutoReversingMigration
    {
        public override void Up()
        {
            CreateTables();
            CreateIndexes();
            CreatePrimaryKeys();
            CreateForeignKeys();
        }

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
                .WithColumn("album_type").AsString()
                .WithColumn("release_date").AsDateTime();

            Create.Table(DataTables.PlaylistTrack)
                .WithColumn("id").AsString().NotNullable()
                .WithTimeStamps()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("track_number").AsInt16().NotNullable()
                .WithColumn("disc_number").AsInt16().NotNullable()
                .WithColumn("added_at").AsDateTime().NotNullable()
                .WithColumn("duration_ms").AsInt64().NotNullable()
                .WithColumn("album_id").AsString().NotNullable()
                .WithColumn("playlist_id").AsString().NotNullable()
                .WithColumn("playlist_snapshot_id").AsString().Nullable();

            Create.Table(DataTables.AlbumArtist)
                .WithTimeStamps()
                .WithColumn("album_id").AsString().NotNullable()
                .WithColumn("artist_id").AsString().NotNullable();

            Create.Table(DataTables.TrackArtist)
                .WithTimeStamps()
                .WithColumn("track_id").AsString().NotNullable()
                .WithColumn("artist_id").AsString().NotNullable();
        }

        // TODO: figure out why these PKs aren't being created
        private void CreatePrimaryKeys()
        {
            Create.PrimaryKey()
                .OnTable(DataTables.AlbumArtist)
                .Columns("album_id", "artist_id");

            Create.PrimaryKey()
                .OnTable(DataTables.TrackArtist)
                .Columns("track_id", "artist_id");

            // The same track could be on multiple playlists
            Create.PrimaryKey()
                .OnTable(DataTables.PlaylistTrack)
                .Columns("id", "playlist_id");
        }

        // TODO: figure out why these FKs aren't being created
        private void CreateForeignKeys()
        {
            Create.ForeignKey()
                .FromTable(DataTables.PlaylistTrack).ForeignColumn("album_id")
                .ToTable(DataTables.Album).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(DataTables.PlaylistTrack).ForeignColumn("playlist_snapshot_id")
                .ToTable(DataTables.PlaylistTrack).PrimaryColumn("snapshot_id");

            Create.ForeignKey()
                .FromTable(DataTables.PlaylistTrack).ForeignColumn("playlist_id")
                .ToTable(DataTables.PlaylistTrack).PrimaryColumn("id");


            Create.ForeignKey()
                .FromTable(DataTables.AlbumArtist).ForeignColumn("album_id")
                .ToTable(DataTables.Album).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(DataTables.AlbumArtist).ForeignColumn("artist_id")
                .ToTable(DataTables.Artist).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(DataTables.TrackArtist).ForeignColumn("track_id")
                .ToTable(DataTables.PlaylistTrack).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(DataTables.TrackArtist).ForeignColumn("artist_id")
                .ToTable(DataTables.Artist).PrimaryColumn("id");
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
