using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Playlister.Configuration;

namespace Playlister.Repositories
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(IOptions<DatabaseOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public SqliteConnection Connection => new(_connectionString);
    }
}
