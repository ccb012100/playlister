using FluentMigrator;

namespace Playlister.Data;

[Migration( 009, "Add AlbumsView view" )]
public class Fm009AddAlbumsView : Migration
{
    public override void Up()
    {
        Execute.Script( "Data/scripts/009_add_albums_view.sql" );
    }

    public override void Down() {
        Execute.Script( "Data/scripts/009_drop_albums_view.sql" );
    }
}
