using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Playlister.Models;

namespace Playlister.Repositories.Implementations
{
    public class PlaylistTrackRepository : IPlaylistTrackRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public PlaylistTrackRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Upsert(MinimalPlaylist playlist, IEnumerable<PlaylistItem> playlistItems,
            CancellationToken ct)
        {
            PlaylistItem[] items = playlistItems as PlaylistItem[] ?? playlistItems.ToArray();
            // Get list of just new tracks so that we don't waste time trying to update stuff that already exists
            ImmutableArray<Track> newTracks = (await GetNewTracks(items)).ToImmutableArray();

            await using SqliteConnection connection = _connectionFactory.Connection;
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);

            await UpsertArtists(newTracks, connection, txn);
            await InsertTracks(newTracks, connection, txn);
            await UpsertPlaylistTracks(playlist, items, connection, txn);
            await UpsertAlbums(newTracks, connection, txn);

            await txn.CommitAsync(ct);

            static async Task UpsertPlaylistTracks(MinimalPlaylist plist, IEnumerable<PlaylistItem> playlistTracks,
                IDbConnection conn, IDbTransaction transaction)
            {
                const string playlistTrackSql =
                    "INSERT INTO PlaylistTrack(track_id, playlist_id, playlist_snapshot_id, added_at) VALUES(@TrackId, @PlaylistId, @SnapshotId, @AddedAt) " +
                    "ON CONFLICT(track_id, playlist_id) DO UPDATE SET playlist_snapshot_id = excluded.playlist_snapshot_id " +
                    "WHERE playlist_snapshot_id != excluded.playlist_snapshot_id";

                // we want to update all Playlist Tracks because the snapshot_id needs to be updated
                await conn.ExecuteAsync(playlistTrackSql,
                    playlistTracks.Select(x => x.ToPlaylistTrack(plist)), transaction);
            }

            static async Task InsertTracks(ImmutableArray<Track> tracks, IDbConnection conn, IDbTransaction transaction)
            {
                const string trackSql =
                    "INSERT INTO Track(id, name, track_number, disc_number, duration_ms, album_id) VALUES(@Id, @Name, @TrackNumber, @DiscNumber, @DurationMs, @AlbumId) " +
                    // only update snapshot_id on conflict, because the rest should be the same
                    "ON CONFLICT(id) DO NOTHING;";

                const string trackArtistSql =
                    "INSERT INTO TrackArtist(track_id, artist_id) VALUES(@TrackId, @ArtistId) " +
                    "ON CONFLICT(track_id, artist_id) DO NOTHING";

                await conn.ExecuteAsync(trackSql, tracks, transaction);
                await conn.ExecuteAsync(trackArtistSql, tracks.Select(x => x.GetArtistIdPairings()), transaction);
            }

            static async Task UpsertAlbums(ImmutableArray<Track> tracks, IDbConnection conn, IDbTransaction transaction)
            {
                const string albumSql =
                    "INSERT INTO Album(id, name, total_tracks, album_type, release_date) VALUES(@Id, @Name, @TotalTracks, @AlbumType, @ReleaseDate) " +
                    "ON CONFLICT(id) IGNORE;";

                ImmutableArray<Album> albums = tracks.Select(t => t.Album).ToImmutableArray();

                await conn.ExecuteAsync(albumSql, albums, transaction);

                const string albumArtistSql =
                    "INSERT INTO AlbumArtist(album_id, artist_id) VALUES(@AlbumId, @ArtistId) " +
                    "ON CONFLICT(album_id, artist_id) DO NOTHING";

                await conn.ExecuteAsync(albumArtistSql, albums.SelectMany(a => a.GetAlbumArtistPairings()),
                    transaction);
            }

            static async Task UpsertArtists(ImmutableArray<Track> tracks, IDbConnection conn,
                IDbTransaction dbTransaction)
            {
                if (dbTransaction == null) throw new ArgumentNullException(nameof(dbTransaction));
                const string artistSql = "INSERT INTO Artist(id, name) VALUES(@Id, @Name) ON CONFLICT(id) DO NOTHING;";
                await conn.ExecuteAsync(artistSql, tracks.Select(x => x.GetAllContainedArtists()),
                    dbTransaction);
            }
        }

        /// <summary>
        /// Get Tracks from input list that are not in the database.
        /// </summary>
        /// <param name="playlistItems"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Track>> GetNewTracks(IEnumerable<PlaylistItem> playlistItems)
        {
            ImmutableArray<Track> tracks = playlistItems.Select(p => p.Track).ToImmutableArray();

            await using SqliteConnection connection = _connectionFactory.Connection;

            // TODO: figure out why this is failing:
            // System.InvalidOperationException: An enumerable sequence of parameters (arrays, lists, etc) is not allowed in this context
            // at Dapper.SqlMapper.GetCacheInfo(Identity identity, Object exampleParameters, Boolean addToCache) in /_/Dapper/SqlMapper.cs:line 1706
            // at Dapper.SqlMapper.QueryAsync[T](IDbConnection cnn, Type effectiveType, CommandDefinition command) in /_/Dapper/SqlMapper.Async.cs:line 410

            IEnumerable<Track> tracksInDb =
                await connection.QueryAsync<Track>("SELECT * FROM Track WHERE id in @Ids", tracks.Select(p => p.Id));

            IEnumerable<string> storedTracks = tracksInDb.Select(t => t.Id);

            return tracks.Where(i => !storedTracks.Contains(i.Id));
        }
    }
}
