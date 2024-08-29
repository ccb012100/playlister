using FluentMigrator;

namespace Playlister.Data;

[Migration(002, "Add modified_at triggers to tables")]
public class Fm002AddDateTriggers : Migration
{
    public override void Up() => Execute.Script("Data/scripts/002_modified_at_triggers.sql");

    public override void Down() { }
}
