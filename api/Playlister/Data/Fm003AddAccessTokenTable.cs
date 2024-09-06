using FluentMigrator;

namespace Playlister.Data;

[Migration( 003, "Add AccessToken table" )]
public class Fm003AddAccessTokenTable : AutoReversingMigration
{
    [Obsolete( "Obsolete" )]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override void Up()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
    {
        Create.Table( DataTables.AccessToken )
            .WithColumn( "access_token" )
            .AsString()
            .NotNullable()
            .PrimaryKey()
            .WithColumn( "refresh_token" )
            .AsString()
            .Nullable()
            .WithColumn( "expiration" )
            .AsDateTime();

        Create.Index()
            .OnTable( DataTables.AccessToken )
            .OnColumn( "expiration" )
            .Descending()
            .WithOptions()
            .Clustered();
    }
}
