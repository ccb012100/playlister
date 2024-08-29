using FluentMigrator;

namespace Playlister.Data;

[Migration(005, "Drop the AccesToken table")]
public class Fm005DropAccessTokenTable : Migration
{
    public override void Up() => Execute.Script("Data/scripts/005_drop_access_token_table.sql");

    public override void Down() { }
}
