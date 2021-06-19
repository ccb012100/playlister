using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210619)]
    // ReSharper disable once UnusedType.Global
    public class Fm20210619AddDateTriggers : Migration
    {
        public override void Up()
        {
            Execute.Script("Data/modified_at_triggers.sql");
        }

        public override void Down()
        {
        }
    }
}
