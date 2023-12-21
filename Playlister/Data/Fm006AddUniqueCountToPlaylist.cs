using FluentMigrator;

namespace Playlister.Data
{
    [Migration(006, description: "Add a 'unique_count' field to Playlist so that we can track the number of unique tracks on a Playlist")]
    public class Fm007AddUniqueCount : AutoReversingMigration
    {
        public override void Up()
        {
            Alter
            .Table(DataTables.Playlist)
                .AddColumn("count_unique")
                .AsInt16()
                .NotNullable()
                .WithDefaultValue(0);
        }
    }
}
