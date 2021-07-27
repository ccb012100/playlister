using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210721)]
    // ReSharper disable once UnusedType.Global
    public class Fm20210721DropAccessTokenTable: Migration
    {
        public override void Up()
        {
            Execute.Script("Data/drop_access_token_table.sql");
        }

        public override void Down()
        {
        }
    }
}