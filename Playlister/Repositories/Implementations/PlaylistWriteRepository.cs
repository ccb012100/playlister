using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Playlister.Models;
using Playlister.Utilities;

namespace Playlister.Repositories.Implementations
{
    public class PlaylistWriteRepository : IPlaylistWriteRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<PlaylistWriteRepository> _logger;

        public PlaylistWriteRepository(
            IConnectionFactory connectionFactory,
            ILogger<PlaylistWriteRepository> logger
        )
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task UpsertAsync(
            Playlist playlist,
            IEnumerable<PlaylistItem> playlistItems,
            CancellationToken ct
        )
        {
            var sw = new Stopwatch();
            sw.Start();

            ImmutableArray<PlaylistItem> items = playlistItems.ToImmutableArray();

            // Get list of just new tracks so that we don't waste time trying to update stuff that already exists
            ImmutableArray<Track> newTracks = await GetNewTracks(items);
            ImmutableArray<Album> albums = newTracks.Select(t => t.Album).ToImmutableArray();

            await using SqliteConnection connection = _connectionFactory.Connection;
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);

            await UpsertPlaylist(playlist, connection, txn);
            await UpsertArtists(newTracks, connection, txn);
            await UpsertAlbums(albums, connection, txn);
            // Track has FK to Album
            await InsertTracks(playlist, newTracks, connection, txn);
            // has FK to Playlist, Track
            await UpsertPlaylistTracks(playlist, items, connection, txn);
            // has FK to Album, Artist
            await UpsertAlbumArtist(albums, connection, txn);
            // has FK to Artist, Track
            await UpsertTrackArtist(newTracks, connection, txn);

            await txn.CommitAsync(ct);

            sw.Stop();
            _logger.LogInformation(
                "\nUpserted {Length} tracks ({Length} new tracks) on Playlist {PlaylistId} {PlaylistName}. Total time: {Elapsed}ms\n",
                items.Length,
                newTracks.Length,
                playlist.Id,
                playlist.Name,
                sw.Elapsed.TotalMilliseconds
            );

            static async Task UpsertPlaylist(
                Playlist plist,
                IDbConnection conn,
                IDbTransaction dbTransaction
            )
            {
                const string sql =
                    "INSERT INTO Playlist(id, snapshot_id, name, collaborative, description, public, count) VALUES(@Id, @SnapshotId, @Name, @Collaborative, @Description, @Public, @Count) "
                    + "ON CONFLICT(id) DO UPDATE SET "
                    + "snapshot_id = excluded.snapshot_id, name = excluded.name, collaborative = excluded.collaborative, public = excluded.public, description = excluded.description, count = excluded.count "
                    + "WHERE snapshot_id != excluded.snapshot_id;";

                await conn.ExecuteAsync(sql, plist, dbTransaction);
            }

            async Task UpsertPlaylistTracks(
                Playlist plist,
                ImmutableArray<PlaylistItem> playlistTracks,
                IDbConnection conn,
                IDbTransaction dbTxn
            )
            {
                _logger.LogDebug(
                    "Attempting to upsert playlist tracks for playlist {PlaylistId} \"{PlaylistName}\"...",
                    plist.Id,
                    plist.Name
                );

                const string playlistTrackSql =
                    "INSERT INTO PlaylistTrack(track_id, playlist_id, playlist_snapshot_id, added_at) VALUES(@TrackId, @PlaylistId, @SnapshotId, @AddedAt) "
                    + "ON CONFLICT(track_id, playlist_id) DO UPDATE SET playlist_snapshot_id = excluded.playlist_snapshot_id "
                    + "WHERE playlist_snapshot_id != excluded.playlist_snapshot_id";

                try
                {
                    await conn.ExecuteAsync(
                        playlistTrackSql,
                        playlistTracks.Select(x => x.ToPlaylistTrack(plist)),
                        dbTxn
                    );
                }
                catch (SqliteException)
                {
                    _logger.LogCritical(
                        "SqliteException thrown while trying to insert tracks into playlist {Playlist}",
                        JsonUtility.PrettyPrint(plist)
                    );
                    throw;
                }
            }

            async Task InsertTracks(
                Playlist plist,
                ImmutableArray<Track> tracks,
                IDbConnection conn,
                IDbTransaction dbTxn
            )
            {
                const string trackSql =
                    "INSERT INTO Track(id, name, track_number, disc_number, duration_ms, album_id) VALUES(@Id, @Name, @TrackNumber, @DiscNumber, @DurationMs, @AlbumId) "
                    +
                    // only update snapshot_id on conflict, because the rest should be the same
                    "ON CONFLICT(id) DO NOTHING;";

                await conn.ExecuteAsync(trackSql, tracks, dbTxn);
                _logger.LogInformation(
                    "Inserted {Length} tracks into Track table from playlist {PlaylistId} \"{PlaylistName}\"",
                    tracks.Length,
                    plist.Id,
                    plist.Name
                );
            }

            static async Task UpsertTrackArtist(
                ImmutableArray<Track> tracks,
                IDbConnection conn,
                IDbTransaction dtTxn
            )
            {
                const string trackArtistSql =
                    "INSERT INTO TrackArtist(track_id, artist_id) VALUES(@TrackId, @ArtistId) "
                    + "ON CONFLICT(track_id, artist_id) DO NOTHING";

                await conn.ExecuteAsync(
                    trackArtistSql,
                    tracks.SelectMany(x => x.GetArtistIdPairings()),
                    dtTxn
                );
            }

            static async Task UpsertAlbums(
                ImmutableArray<Album> albums,
                IDbConnection conn,
                IDbTransaction dbTxn
            )
            {
                const string albumSql =
                    "INSERT INTO Album(id, name, total_tracks, album_type, release_date) VALUES(@Id, @Name, @TotalTracks, @AlbumType, @ReleaseDate) "
                    + "ON CONFLICT(id) DO NOTHING;";

                await conn.ExecuteAsync(albumSql, albums, dbTxn);
            }

            static async Task UpsertArtists(
                ImmutableArray<Track> tracks,
                IDbConnection conn,
                IDbTransaction dbTxn
            )
            {
                IEnumerable<Artist> artists = tracks.SelectMany(x => x.GetAllContainedArtists());

                const string artistSql =
                    "INSERT INTO Artist(id, name) VALUES(@Id, @Name) ON CONFLICT(id) DO NOTHING;";
                await conn.ExecuteAsync(artistSql, artists, dbTxn);
            }

            static async Task UpsertAlbumArtist(
                ImmutableArray<Album> albums,
                IDbConnection conn,
                IDbTransaction dbTxn
            )
            {
                const string albumArtistSql =
                    "INSERT INTO AlbumArtist(album_id, artist_id) VALUES(@AlbumId, @ArtistId) "
                    + "ON CONFLICT(album_id, artist_id) DO NOTHING";

                await conn.ExecuteAsync(
                    albumArtistSql,
                    albums.SelectMany(a => a.GetAlbumArtistPairings()),
                    dbTxn
                );
            }

            async Task<ImmutableArray<Track>> GetNewTracks(ImmutableArray<PlaylistItem> listItems)
            {
                ImmutableArray<Track> tracks = listItems.Select(p => p.Track).ToImmutableArray();

                await using SqliteConnection conn = _connectionFactory.Connection;

                IEnumerable<string> ids = tracks.Select(p => p.Id);

                var parameters = new DynamicParameters();
                parameters.Add("@Ids", ids);

                IEnumerable<string> tracksInDb = await conn.QueryAsync<string>(
                    "SELECT id FROM Track WHERE id in @Ids",
                    parameters
                );

                return tracks.Where(i => !tracksInDb.Contains(i.Id)).ToImmutableArray();
            }
        }
    }
}
