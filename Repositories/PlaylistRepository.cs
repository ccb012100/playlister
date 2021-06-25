using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Utilities;

namespace Playlister.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ILogger<PlaylistRepository> _logger;
        private readonly string _connectionString;

        private static readonly CacheObject<Playlist> PlaylistCache = new();

        public PlaylistRepository(IOptions<DatabaseOptions> options, ILogger<PlaylistRepository> logger)
        {
            _logger = logger;
            _connectionString = options.Value.ConnectionString;

            PlaylistCache.Initialize(PopulateCache);
        }

        public async Task Upsert(IEnumerable<SimplifiedPlaylistObject> playlists, CancellationToken ct)
        {
            // get all stored playlists
            IEnumerable<Playlist> allLists = PlaylistCache.Items.Select(x => x.Value).ToImmutableArray();

            // only update lists that don't have an entry or that have a changed snapshot_id
            List<SimplifiedPlaylistObject> changedLists = playlists
                .Where(pl => allLists.All(a => a.Id != pl.Id || a.SnapshotId != pl.SnapshotId)).ToList();

            const string playlistSql =
                "INSERT INTO Playlist(id, snapshot_id, name, collaborative, description, public) VALUES(@Id, @SnapshotId, @Name, @Collaborative, @Description, @Public) " +
                "ON CONFLICT(id) DO UPDATE SET " +
                "snapshot_id = excluded.snapshot_id, name = excluded.name, collaborative = excluded.collaborative, public = excluded.public " +
                "WHERE snapshot_id != excluded.snapshot_id;";

            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);

            await connection.ExecuteAsync(playlistSql, playlists, transaction: txn);

            await txn.CommitAsync(ct);

            // Cache updated lists
            foreach (SimplifiedPlaylistObject list in changedLists)
            {
                Cache(list.ToPlaylist());
            }
        }

        public IEnumerable<Playlist> GetAll() => PlaylistCache.Items.Select(p => p.Value);

        public Playlist? Get(string id) => GetFromCache(id);

        #region cache

        private void Cache(Playlist playlist)
        {
            Playlist? pl = PlaylistCache.Items.AddOrUpdate(playlist.Id, playlist,
                (_, b) => b == null ? throw new ArgumentNullException(nameof(b)) : playlist);

            _logger.LogDebug($"Added token to cache: {JsonUtility.PrettyPrint(pl)}");
        }

        private Playlist? GetFromCache(string id)
        {
            _ = PlaylistCache.Items.TryGetValue(id, out Playlist? playlist);
            _logger.LogDebug($"Result of getting playlist {id} from cache: {JsonUtility.PrettyPrint(playlist)}");
            return playlist;
        }

        private async Task PopulateCache()
        {
            try
            {
                await using var conn = new SqliteConnection(_connectionString);
                IEnumerable<Playlist> playlists =
                    await conn.QueryAsync<Playlist>("SELECT * FROM Playlist");

                _logger.LogDebug("Populating Playlist cache...");
                foreach (Playlist? playlist in playlists)
                {
                    Cache(playlist);
                    _logger.LogDebug($"Added playlist to cache: {JsonUtility.PrettyPrint(playlist)}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }

            _logger.LogDebug($"Cache populated: {JsonUtility.PrettyPrint(PlaylistCache.Items)}");
        }

        #endregion
    }
}
