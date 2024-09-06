using FluentMigrator;

namespace Playlister.Data;

[Migration( 008, "Create new tables for ExcludedIds and Saved Albums" )]
public class Fm008AddAlbumFields : Migration
{
    public override void Up()
    {
        Create.Table( DataTables.SavedAlbum )
            .WithSpotifyPrimaryIdColumn()
            .WithTimeStamps()
            .WithColumn( "label" )
            .AsString()
            .NotNullable()
            .ForeignKey( "fk_savedalbum_album", DataTables.Album, "id" );

        Create.Table( DataTables.ExternalId )
            .WithColumn( "album_id" )
            .AsString()
            .NotNullable()
            .PrimaryKey()
            .WithTimeStamps()
            .WithColumn( "ean" )
            .AsString()
            .Nullable()
            .WithColumn( "isrc" )
            .AsString()
            .Nullable()
            .WithColumn( "upc" )
            .AsString()
            .Nullable()
            .ForeignKey( "fk_externalid_album", DataTables.SavedAlbum, "id" );

        Create.Index().OnTable( DataTables.SavedAlbum ).OnColumn( "id" ).Ascending().WithOptions().Clustered();
        Create.Index().OnTable( DataTables.ExternalId ).OnColumn( "album_id" ).Ascending().WithOptions().Clustered();
    }

    public override void Down()
    {
    }
}
