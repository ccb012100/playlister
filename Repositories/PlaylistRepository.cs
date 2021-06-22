using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly string _connectionString;

        public PlaylistRepository(IOptions<DatabaseOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task Upsert(IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct)
        {
            const string sql =
                "INSERT INTO Playlist(id, snapshot_id, name, collaborative, description, public) VALUES(@Id, @SnapshotId, @Name, @Collaborative, @Description, @Public) " +
                "ON CONFLICT(id) DO UPDATE SET " +
                "snapshot_id = excluded.snapshot_id, name = excluded.name, collaborative = excluded.collaborative, public = excluded.public " +
                "WHERE snapshot_id != excluded.snapshot_id;";

            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);
            await connection.ExecuteAsync(sql, playlists, transaction: txn);
            await txn.CommitAsync(ct);
        }

        public async Task<IEnumerable<Playlist>> Get()
        {
            await using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<Playlist>("SELECT * FROM Playlist");
        }

        public async Task<Playlist> Get(string id)
        {
            await using var conn = new SqliteConnection(_connectionString);
            return await conn.QuerySingleOrDefaultAsync<Playlist?>("SELECT * FROM Playlist WHERE id = @id") ??
                   throw new DbRecordNotFoundException($"Playlist {nameof(id)} = `{id}` not found.");
        }
    }
}
