using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Playlister.Extensions;
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

        public async Task DeleteOrphanedPlaylistTracksAsync(CancellationToken ct)
        {
            Stopwatch sw = new();
            sw.Start();

            await using SqliteConnection connection = _connectionFactory.Connection;
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);

            int deleted = int.MinValue;

            try
            {
                deleted = await connection.ExecuteScalarQueryAsync(
                     SqlQueries.Delete.OrphanedTracks,
                     txn
                 );
            }
            catch (SqliteException)
            {
                _logger.LogCritical("SqliteException thrown while trying to delete orphaned PlaylistTracks");

                throw;
            }
            finally
            {
                sw.Stop();
                await txn.CommitAsync(ct);
            }

            _logger.LogInformation(
                "Deleted {Deleted} orphaned PlaylistTracks from the DB. Total time: {Elapsed}",
                deleted,
                sw.Elapsed.ToLogString()
            );
        }

        public async Task UpsertAsync(
            Playlist playlist,
            IEnumerable<PlaylistItem> playlistItems,
            CancellationToken ct
        )
        {
            await using SqliteConnection connection = _connectionFactory.Connection;
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);

            try
            {
                Stopwatch sw = new();
                sw.Start();

                /*
                 *  Due to FKs, these have to be upserted in a certain order
                 */

                await UpsertPlaylist(playlist, connection, txn);

                // convert to avoid multiple enumerations
                ImmutableArray<PlaylistItem> items = playlistItems.ToImmutableArray();

                // The DB calls will blow up if the collection is empty
                if (playlistItems.Any())
                {
                    await UpsertPlaylistItems(playlist, items, connection, txn);
                }

                sw.Stop();

                _logger.LogInformation(
                    "{PlaylistTag} Upserted {Upserted} tracks. Total time: {Elapsed}",
                    playlist.LoggingTag,
                    items.Length,
                    sw.Elapsed.ToLogString()
                );
            }
            catch (SqliteException)
            {
                _logger.LogCritical(
                    "{PlaylistTag} SqliteException thrown while trying to upsert data for playlist: {PlaylistData}",
                    playlist.LoggingTag,
                    JsonUtility.PrettyPrint(playlist)
                );

                throw;
            }
            finally
            {
                await txn.CommitAsync(ct);
            }

            async Task UpsertPlaylistItems(Playlist playlist, ImmutableArray<PlaylistItem> items, SqliteConnection connection, DbTransaction txn)
            {
                ImmutableArray<Track> tracks = items.Select(t => t.Track).ToImmutableArray();
                ImmutableArray<Album> albums = tracks.Select(t => t.Album).ToImmutableArray();

                List<Task> tasks = new()
                    {
                        UpsertArtists(tracks, connection, txn),
                        UpsertAlbums(albums, connection, txn)
                    };

                await Task.WhenAll(tasks);

                tasks.Clear();

                // Track has FK to Album
                await UpsertTracks(playlist, tracks, connection, txn);

                // has FK to Playlist, Track
                tasks.Add(UpsertPlaylistTracks(playlist, items, connection, txn));
                // has FK to Album, Artist
                tasks.Add(UpsertAlbumArtists(albums, connection, txn));
                // has FK to Artist, Track
                tasks.Add(UpsertTrackArtist(tracks, connection, txn));

                await Task.WhenAll(tasks);
            }

            async Task UpsertPlaylist(
                Playlist playlist,
                IDbConnection conn,
                IDbTransaction dbTransaction
            )
            {
                await conn.UpsertAsync(SqlQueries.Upsert.Playlist, playlist, dbTransaction);

                _logger.LogInformation("{PlaylistTag} Upserted Playlist",
                    playlist.LoggingTag
                );
            }

            async Task UpsertPlaylistTracks(
                Playlist playlist,
                ImmutableArray<PlaylistItem> playlistItems,
                IDbConnection conn,
                IDbTransaction dbTxn
            )
            {
                _logger.LogDebug(
                    "{PlaylistTag} Attempting to upsert {Count} PlaylistTracks...",
                    playlist.LoggingTag,
                    playlistItems.Length
                );

                ImmutableArray<PlaylistTrack> playlistTracks = playlistItems.Select(x => x.ToPlaylistTrack(playlist)).ToImmutableArray();

                await conn.UpsertAsync(SqlQueries.Upsert.PlaylistTrack, playlistTracks, dbTxn);

                _logger.LogInformation("{PlaylistTag} Upserted {Upserted} PlaylistTracks",
                    playlist.LoggingTag,
                    playlistTracks.Length
                );
            }

            async Task UpsertTracks(
                Playlist playlist,
                ImmutableArray<Track> tracks,
                IDbConnection conn,
                IDbTransaction dbTxn
            )
            {
                await conn.UpsertAsync(SqlQueries.Upsert.Track, tracks, dbTxn);

                _logger.LogInformation(
                    "{PlaylistTag} Upserted {Upserted} Tracks",
                    playlist.LoggingTag,
                    tracks.Length
                );
            }

            async Task UpsertTrackArtist(
               ImmutableArray<Track> tracks,
               IDbConnection conn,
               IDbTransaction dtTxn
           )
            {
                ImmutableArray<object> trackArtists = tracks.SelectMany(x => x.GetArtistIdPairings()).ToImmutableArray();

                await conn.UpsertAsync(SqlQueries.Upsert.TrackArtist, trackArtists, dtTxn);

                _logger.LogInformation(
                    "{PlaylistTag} Upserted {Upserted} TrackArtists",
                    playlist.LoggingTag,
                    trackArtists.Length
                );
            }

            async Task UpsertAlbums(
               ImmutableArray<Album> albums,
               IDbConnection conn,
               IDbTransaction dbTxn
           )
            {
                await conn.UpsertAsync(SqlQueries.Upsert.Album, albums, dbTxn);

                _logger.LogInformation(
                    "{PlaylistTag} Upserted {Upserted} Albums",
                    playlist.LoggingTag,
                    albums.Length
                );
            }

            async Task UpsertArtists(
               ImmutableArray<Track> tracks,
               IDbConnection conn,
               IDbTransaction dbTxn
           )
            {
                ImmutableArray<Artist> artists = tracks.SelectMany(x => x.GetAllContainedArtists()).ToImmutableArray();

                await conn.UpsertAsync(SqlQueries.Upsert.Artist, artists, dbTxn);

                _logger.LogInformation(
                    "{PlaylistTag} Upserted {Upserted} Artists",
                    playlist.LoggingTag,
                    artists.Length
                );
            }

            async Task UpsertAlbumArtists(
               ImmutableArray<Album> albums,
               IDbConnection conn,
               IDbTransaction dbTxn
           )
            {
                var albumArtists = albums.SelectMany(a => a.GetAlbumArtistPairings()).ToImmutableArray();

                await conn.UpsertAsync(SqlQueries.Upsert.AlbumArtist, albumArtists, dbTxn);

                _logger.LogInformation(
                    "{PlaylistTag} Upserted {Upserted} AlbumArtists",
                    playlist.LoggingTag,
                    albumArtists.Length
                );
            }
        }
    }
}
