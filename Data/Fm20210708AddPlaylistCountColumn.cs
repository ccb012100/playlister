using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210708)]
    // ReSharper disable once UnusedType.Global
    public class Fm20210708AddPlaylistCountColumn: AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table(DataTables.Playlist).AddColumn("count").AsInt16().NotNullable().WithDefaultValue(0);
        }
    }
}
