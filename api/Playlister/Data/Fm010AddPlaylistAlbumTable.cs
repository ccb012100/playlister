using FluentMigrator;

namespace Playlister.Data;

[Migration( 010, "Add PlaylistAlbum table" )]
public class Fm010AddPlaylistAlbumTable : Migration
{
    public override void Up()
    {
        Execute.Script( "Data/scripts/010_add_playlistalbum_table.sqlite" );
    }

    public override void Down()
    {
        Execute.Script( "Data/scripts/010_drop_playlistalbum_table.sqlite" );
    }
}
