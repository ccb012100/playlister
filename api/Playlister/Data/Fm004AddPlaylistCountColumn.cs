using FluentMigrator;

namespace Playlister.Data
{
    [Migration(004, "Add 'count' columne to the Playlist table")]
    public class Fm004AddPlaylistCountColumn : AutoReversingMigration
    {
        public override void Up() =>
            Alter
                .Table(DataTables.Playlist)
                .AddColumn("count")
                .AsInt16()
                .NotNullable()
                .WithDefaultValue(0);
    }
}
