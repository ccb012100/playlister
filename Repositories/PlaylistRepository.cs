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

        /// <summary>
        /// Create playlist.
        /// If playlist exists, update the entry IFF <c>snapshot_id</c> differs.
        /// </summary>
        /// <param name="playlists"></param>
        /// <param name="ct"></param>
        public async Task Upsert(IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct)
        {
            const string sql =
                "INSERT INTO Playlist(id, snapshot_id, name, collaborative, description, public) " +
                "VALUES(@Id, @SnapshotId, @Name, @Collaborative, @Description, @Public) " +
                "ON CONFLICT(id) DO UPDATE SET " +
                "snapshot_id = excluded.snapshot_id, name = excluded.name, collaborative = excluded.collaborative, public = excluded.public " +
                "WHERE snapshot_id != excluded.snapshot_id;";

            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);
            await connection.ExecuteAsync(sql, playlists, transaction: txn);
            await txn.CommitAsync(ct);
        }

        /// <summary>
        /// Get all Playlists from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Playlist>> Get()
        {
            const string sql = "SELECT * FROM Playlist";
            await using var connection = new SqliteConnection(_connectionString);

            IEnumerable<Playlist> playlists = await connection.QueryAsync<Playlist>(sql);

            return playlists;
        }

        public async Task<Playlist> Get(string id)
        {
            const string sql = "SELECT * FROM Playlist WHERE id = @id";
            await using var conn = new SqliteConnection(_connectionString);

            var playlist = await conn.QuerySingleOrDefaultAsync<Playlist?>(sql);

            if (playlist is null) throw new DbRecordNotFoundException($"Playlist {nameof(id)} = `{id}` not found.");

            return playlist;
        }
    }
}
