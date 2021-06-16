using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210616)]
    // ReSharper disable once UnusedType.Global
    public class AddPlaylistTable : Migration
    {
        private const string Playlist = "Playlist";
        private const string Track = "Track";
        private const string Album = "Album";
        private const string Artist = "Artist";
        private const string AlbumArtist = "AlbumArtist";
        private const string TrackArtist = "TrackArtist";

        public override void Up()
        {
            /* Playlist table */
            Create.Table(Playlist)
                .WithColumn("id").AsString().NotNullable().PrimaryKey() // size?
                .WithColumn("snapshot_id").AsString().Unique().NotNullable() // size?
                .WithColumn("collaborative").AsBoolean().NotNullable()
                .WithColumn("description").AsString().Nullable();

            /* Track table */
            Create.Table(Track)
                .WithColumn("id").AsString().NotNullable().PrimaryKey() // size?
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("track_number").AsInt16().NotNullable()
                .WithColumn("disc_number").AsInt16().NotNullable()
                .WithColumn("added_at").AsDateTime().NotNullable()
                .WithColumn("duration_ms").AsInt64().NotNullable();

            Create.Index("ix_added_at")
                .OnTable(Track)
                .OnColumn("added_at").Descending()
                .WithOptions().Clustered();

            /* Artist table */
            Create.Table(Artist)
                .WithColumn("id").AsString().NotNullable().PrimaryKey() // size?
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("url").AsString().NotNullable();

            /* Album table */
            Create.Table(Album)
                .WithColumn("id").AsString().NotNullable().PrimaryKey() // size?
                .WithColumn("total_tracks").AsInt16().NotNullable()
                .WithColumn("album_type").AsString(50)
                .WithColumn("release_date").AsDateTime()
                .WithColumn("url").AsString().NotNullable();

            Create.Index("ix_release_date")
                .OnTable(Album)
                .OnColumn("total_tracks").Ascending()
                .WithOptions().Clustered();

            /* AlbumArtist table for many-to-many Artist/Album relationship */
            Create.Table(AlbumArtist)
                .WithColumn("album_id").AsString().NotNullable()
                .WithColumn("artist_id").AsString().NotNullable();

            Create.PrimaryKey("PK_AlbumArtist")
                .OnTable(AlbumArtist)
                .Columns("album_id", "artist_id");

            Create.ForeignKey()
                .FromTable(AlbumArtist).ForeignColumn("album_id")
                .ToTable(Album).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(AlbumArtist).ForeignColumn("artist_id")
                .ToTable(Artist).PrimaryColumn("id");

            /* TrackArtist table for many-to-many Track/Artist relationship */
            Create.Table(TrackArtist)
                .WithColumn("track_id").AsString().NotNullable()
                .WithColumn("artist_id").AsString().NotNullable();

            Create.PrimaryKey("PK_TrackArtist")
                .OnTable(TrackArtist)
                .Columns("track_id", "artist_id");

            Create.ForeignKey()
                .FromTable(TrackArtist).ForeignColumn("track_id")
                .ToTable(Track).PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable(TrackArtist).ForeignColumn("artist_id")
                .ToTable(Artist).PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.Table(Artist);
            Delete.Table(Album);
            Delete.Table(Track);
            Delete.Table(Playlist);
        }
    }
}
