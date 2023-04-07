using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210621)]
    public class Fm20210621AddDateTriggers : Migration
    {
        public override void Up()
        {
            Execute.Script("Data/modified_at_triggers.sql");
        }

        public override void Down() { }
    }
}
