using Dapper;

using Microsoft.Data.Sqlite;

namespace Playlister.Tests.Integration;

/// <summary>
///     Helper class for initializing test databases with schema and seed data.
/// </summary>
public static class TestDatabaseHelper {
    private static readonly string SqliteFolder = Path.Combine( Directory.GetCurrentDirectory( ) , "SQLite" );
    private static readonly Lazy<string> SchemaScript = new( ( ) => File.ReadAllText( Path.Combine( SqliteFolder , "create_db.sqlite" ) ) );
    private static readonly Lazy<string> SeedScript = new( ( ) => File.ReadAllText( Path.Combine( SqliteFolder , "seed_db.sql" ) ) );

    /// <summary>
    ///     Initialize the database with schema only (no seed data).
    /// </summary>
    /// <param name="connection">The SQLite connection to initialize.</param>
    public static async Task InitializeSchemaAsync( SqliteConnection connection ) {
        await connection.OpenAsync( );
        await connection.ExecuteAsync( SchemaScript.Value );
    }

    /// <summary>
    ///     Initialize the database with schema and seed data.
    /// </summary>
    /// <param name="connection">The SQLite connection to initialize.</param>
    public static async Task InitializeWithSeedDataAsync( SqliteConnection connection ) {
        await InitializeSchemaAsync( connection );
        await connection.ExecuteAsync( SeedScript.Value );
    }

    /// <summary>
    ///     Initialize the database with schema only (no seed data). Synchronous version.
    /// </summary>
    /// <param name="connection">The SQLite connection to initialize.</param>
    public static void InitializeSchema( SqliteConnection connection ) {
        connection.Open( );
        connection.Execute( SchemaScript.Value );
    }

    /// <summary>
    ///     Initialize the database with schema and seed data. Synchronous version.
    /// </summary>
    /// <param name="connection">The SQLite connection to initialize.</param>
    public static void InitializeWithSeedData( SqliteConnection connection ) {
        InitializeSchema( connection );
        connection.Execute( SeedScript.Value );
    }
}
