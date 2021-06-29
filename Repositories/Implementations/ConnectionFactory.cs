using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Playlister.Configuration;

namespace Playlister.Repositories.Implementations
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(IOptions<DatabaseOptions> options)
        {
            _connectionString = new SqliteConnectionStringBuilder(options.Value.ConnectionString)
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared,
                ForeignKeys = true
            }.ToString();

            Console.WriteLine("CONNECTION STRING:\n" + _connectionString);
        }

        public SqliteConnection Connection => new(_connectionString);
    }
}
