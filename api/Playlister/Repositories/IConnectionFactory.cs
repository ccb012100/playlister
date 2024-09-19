using Microsoft.Data.Sqlite;

namespace Playlister.Repositories;

public interface IConnectionFactory {
    SqliteConnection Connection { get; }
}
