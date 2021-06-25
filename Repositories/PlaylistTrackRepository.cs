using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Models;

namespace Playlister.Repositories
{
    public class PlaylistTrackRepository : IPlaylistTrackRepository
    {
        private readonly string _connectionString;

        public PlaylistTrackRepository(IOptions<DatabaseOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task Upsert(MinimalPlaylist playlist, IEnumerable<PlaylistItem> tracks, CancellationToken ct)
        {
            const string sql =
                @"INSERT INTO PlaylistTrack(id, name, track_number, disc_number, added_at, duration_ms, album_id, playlist_id, playlist_snapshot_id) VALUES(@Id, @Name, @TrackNumber, @DiscNumber, @AddedAt, @DurationMs, @AlbumId, @PlaylistId, @SnapshotId) " +
                // only update snapshot_id on conflict, because the rest should be the same
                "ON CONFLICT(id) DO UPDATE SET playlist_id = excluded.playlist_id " +
                "WHERE playlist_snapshot_id != excluded.playlist_snapshot_id;";

            // TODO: update Artists
            // TODO: upsert Album

            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);

            (string playlistId, string? snapshotId) = playlist;

            await connection.ExecuteAsync(sql,
                tracks.Select(x => new
                {
                    x.Track.Id,
                    x.Track.Name,
                    x.Track.TrackNumber,
                    x.Track.DiscNumber,
                    x.AddedAt,
                    x.Track.DurationMs,
                    AlbumId = x.Track.Album.Id,
                    PlaylistId = playlistId,
                    SnapshotId = snapshotId
                }),
                transaction: txn);

            await txn.CommitAsync(ct);
        }
    }
}
