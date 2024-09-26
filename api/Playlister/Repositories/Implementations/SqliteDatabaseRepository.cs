using System.Diagnostics;

using Microsoft.Data.Sqlite;

namespace Playlister.Repositories.Implementations;

public class SqliteDatabaseRepository( ILogger<SqliteDatabaseRepository> logger , IConnectionFactory connectionFactory ) : ISqliteDatabaseRepository {
    private readonly IConnectionFactory _connectionFactory = connectionFactory;
    private readonly ILogger<SqliteDatabaseRepository> _logger = logger;

    public async Task VacuumDatabase( CancellationToken ct ) {
        Stopwatch sw = new( );
        sw.Start( );

        await using SqliteConnection connection = _connectionFactory.Connection;
        await connection.OpenAsync( ct );

        try {
            SqliteCommand sqliteCommand = new( "VACUUM" , connection );
            await sqliteCommand.ExecuteNonQueryAsync( ct );

            _logger.LogInformation( "Vacuumed the database. Total time: {Elapsed}" , sw.Elapsed.ToDisplayString( ) );
        } catch ( SqliteException e ) {
            _logger.LogCritical( e , "{ExceptionType} thrown while trying to vacuum the database" , nameof( SqliteException ) );

            throw;
        } finally {
            sw.Stop( );
        }
    }
}
