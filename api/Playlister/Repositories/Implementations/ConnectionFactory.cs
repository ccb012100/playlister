using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

using Playlister.Configuration;

namespace Playlister.Repositories.Implementations;

public class ConnectionFactory( IOptions<DatabaseOptions> options ) : IConnectionFactory {
    private readonly string _connectionString = new SqliteConnectionStringBuilder( options.Value.ConnectionString ) {
        Mode = SqliteOpenMode.ReadWriteCreate ,
        Cache = SqliteCacheMode.Shared ,
        ForeignKeys = true
    }.ToString( );

    public SqliteConnection Connection => new( _connectionString );
}
