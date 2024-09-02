using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Playlister.Configuration;

namespace Playlister.Repositories.Implementations;

public class ConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public ConnectionFactory( IOptions<DatabaseOptions> options )
    {
        _connectionString = new SqliteConnectionStringBuilder( options.Value.ConnectionString )
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            ForeignKeys = true
        }.ToString();
    }

    public SqliteConnection Connection => new(_connectionString);
}
