using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace Playlister.Data;

internal static class FluentMigratorExtensions {
    /// <summary>
    ///     Add a Spotify <c>id</c> string column to the table and set as Primary Key.
    /// </summary>
    /// <param name="tableWithColumnSyntax">The table to add the TrackId column to</param>
    /// <returns></returns>
    public static ICreateTableColumnOptionOrWithColumnSyntax WithSpotifyPrimaryIdColumn(
        this ICreateTableWithColumnSyntax tableWithColumnSyntax
    ) {
        return tableWithColumnSyntax
            .WithColumn( "id" )
            .AsString( )
            .NotNullable( )
            .PrimaryKey( );
    }

    /// <summary>
    ///     Upsert <c>created_at</c> and <c>modified_at</c> columns on the table.
    /// </summary>
    /// <param name="tableWithColumnSyntax">The table to add the columns to</param>
    /// <returns></returns>
    public static ICreateTableColumnOptionOrWithColumnSyntax WithTimeStamps(
        this ICreateTableWithColumnSyntax tableWithColumnSyntax
    ) {
        return tableWithColumnSyntax
            .WithColumn( "created_at" )
            .AsDateTime( )
            .NotNullable( )
            .WithDefault( SystemMethods.CurrentUTCDateTime )
            .WithColumn( "modified_at" )
            .AsDateTime( )
            .Nullable( );
    }
}
