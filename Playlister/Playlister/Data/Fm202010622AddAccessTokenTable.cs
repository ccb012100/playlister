using FluentMigrator;

namespace Playlister.Data
{
    [Migration(20210622)]
    public class Fm202010622AddAccessTokenTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(DataTables.AccessToken)
                .WithColumn("access_token").AsString().NotNullable().PrimaryKey()
                .WithColumn("refresh_token").AsString().Nullable()
                .WithColumn("expiration").AsDateTime();

            Create.Index().OnTable(DataTables.AccessToken)
                .OnColumn("expiration").Descending()
                .WithOptions().Clustered();
        }
    }
}
