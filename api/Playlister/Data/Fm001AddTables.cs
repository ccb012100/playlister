using FluentMigrator;

namespace Playlister.Data;

[Migration( 001, "Create the database" )]
public class Fm001CreateDb : Migration
{
    public override void Up()
    {
        Execute.Script( "Data/scripts/fm001_create_db.sqlite" );
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
