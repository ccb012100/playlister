using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Utilities;

namespace Playlister.Repositories.Implementations;

public class PlaylistWriteRepository(
    IConnectionFactory connectionFactory,
    ILogger<PlaylistWriteRepository> logger
    ) : IPlaylistWriteRepository
{
    private readonly IConnectionFactory _connectionFactory = connectionFactory;
    private readonly ILogger<PlaylistWriteRepository> _logger = logger;

    public async Task<int> DeleteOrphanedPlaylistTracksAsync( CancellationToken ct )
    {
        Stopwatch sw = new();
        sw.Start();

        await using SqliteConnection connection = _connectionFactory.Connection;
        await connection.OpenAsync( ct );
        DbTransaction txn = await connection.BeginTransactionAsync( ct );

        try
        {
            int deleted = await connection.ExecuteScalarQueryAsync(
                SqlQueries.Delete.OrphanedTracks,
                txn
            );

            _logger.LogInformation(
                "Deleted {Deleted} orphaned PlaylistTracks from the DB. Total time: {Elapsed}",
                deleted,
                sw.Elapsed.ToDisplayString()
            );

            return deleted;
        }
        catch (SqliteException)
        {
            _logger.LogCritical( "SqliteException thrown while trying to delete orphaned PlaylistTracks" );

            throw;
        }
        finally
        {
            sw.Stop();
            await txn.CommitAsync( ct );
        }
    }

    public async Task UpsertAsync(
        Playlist playlist,
        IEnumerable<PlaylistItem> playlistItems,
        CancellationToken ct
    )
    {
        await using SqliteConnection connection = _connectionFactory.Connection;
        await connection.OpenAsync( ct );
        DbTransaction txn = await connection.BeginTransactionAsync( ct );

        try
        {
            Stopwatch sw = new();
            sw.Start();

            /*
             *  Due to FKs, these have to be upserted in a certain order
             */

            await UpsertPlaylist( playlist, connection, txn );

            // convert to avoid multiple enumerations
            ImmutableArray<PlaylistItem> items = playlistItems.ToImmutableArray();

            // The DB calls will blow up if the collection is empty
            if (items.Length > 0)
            {
                await UpsertPlaylistItems( playlist, items, connection, txn );
                sw.Stop();

                _logger.LogInformation(
                    "{PlaylistTag} Upserted {Upserted} tracks. Total time: {Elapsed}",
                    playlist.LoggingTag,
                    items.Length,
                    sw.Elapsed.ToDisplayString()
                );

                return;
            }

            _logger.LogDebug( "{PlaylistTag} Upserted 0 tracks", playlist.LoggingTag );

            sw.Stop();
        }
        catch (SqliteException)
        {
            _logger.LogCritical(
                "{PlaylistTag} SqliteException thrown while trying to upsert data for playlist: {PlaylistData}",
                playlist.LoggingTag,
                playlist.PrettyPrint()
            );

            throw;
        }
        finally
        {
            await txn.CommitAsync( ct );
        }

        return;

        async Task UpsertPlaylistItems( Playlist plist, ImmutableArray<PlaylistItem> items, SqliteConnection conn, DbTransaction transaction )
        {
            ImmutableArray<Track> tracks = items.Select( t => t.Track ).ToImmutableArray();
            ImmutableArray<Album> albums = tracks.Select( t => t.Album ).ToImmutableArray();

            List<Task> tasks = new()
            {
                UpsertArtists( tracks, conn, transaction ),
                UpsertAlbums( albums, conn, transaction )
            };

            await Task.WhenAll( tasks );

            tasks.Clear();

            await UpsertTracks( plist, tracks, conn, transaction ); // Track has FK to Album

            /*
             * Due to Foreign Keys, order of operations is important
             */
            tasks.Add( UpsertPlaylistTracks( plist, items, conn, transaction ) ); // has FK to Playlist, Track
            tasks.Add( UpsertAlbumArtists( albums, conn, transaction ) ); // has FK to Album, Artist
            tasks.Add( UpsertTrackArtist( tracks, conn, transaction ) ); // has FK to Artist, Track

            await Task.WhenAll( tasks );
        }

        async Task UpsertPlaylist(
            Playlist plist,
            IDbConnection conn,
            IDbTransaction dbTxn
        )
        {
            await conn.UpsertAsync( SqlQueries.Upsert.Playlist, plist, dbTxn );

            _logger.LogDebug(
                "{PlaylistTag} Upserted Playlist {Playlist} {PlaylistId}",
                plist.LoggingTag,
                plist.Name,
                plist.Id
            );
        }

        async Task UpsertPlaylistTracks(
            Playlist plist,
            ImmutableArray<PlaylistItem> plistItems,
            IDbConnection conn,
            IDbTransaction dbTxn
        )
        {
            _logger.LogTrace(
                "{PlaylistTag} Attempting to upsert {Count} PlaylistTracks...",
                plist.LoggingTag,
                plistItems.Length
            );

            ImmutableArray<PlaylistTrack> playlistTracks = plistItems.Select( x => x.ToPlaylistTrack( plist ) ).ToImmutableArray();

            await conn.UpsertAsync( SqlQueries.Upsert.PlaylistTrack, playlistTracks, dbTxn );

            _logger.LogDebug(
                "{PlaylistTag} Upserted {Upserted} PlaylistTracks",
                plist.LoggingTag,
                playlistTracks.Length
            );
        }

        async Task UpsertTracks(
            Playlist plist,
            ImmutableArray<Track> tracks,
            IDbConnection conn,
            IDbTransaction dbTxn
        )
        {
            await conn.UpsertAsync( SqlQueries.Upsert.Track, tracks, dbTxn );

            _logger.LogDebug(
                "{PlaylistTag} Upserted {Upserted} Tracks",
                plist.LoggingTag,
                tracks.Length
            );
        }

        async Task UpsertTrackArtist(
            ImmutableArray<Track> tracks,
            IDbConnection conn,
            IDbTransaction dtTxn
        )
        {
            ImmutableArray<Track.ArtistTrackIdPair> trackArtists = tracks.SelectMany( x => x.GetArtistTrackIdPairings() ).ToImmutableArray();

            await conn.UpsertAsync( SqlQueries.Upsert.TrackArtist, trackArtists, dtTxn );

            _logger.LogDebug(
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
            await conn.UpsertAsync( SqlQueries.Upsert.Album, albums, dbTxn );

            _logger.LogDebug(
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
            ImmutableArray<Artist> artists = tracks.SelectMany( x => x.GetAllContainedArtists() ).ToImmutableArray();

            await conn.UpsertAsync( SqlQueries.Upsert.Artist, artists, dbTxn );

            _logger.LogDebug(
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
            ImmutableArray<AlbumArtistPair> albumArtists = albums.SelectMany( a => a.GetAlbumArtistPairings() ).ToImmutableArray();

            await conn.UpsertAsync( SqlQueries.Upsert.AlbumArtist, albumArtists, dbTxn );

            _logger.LogDebug(
                "{PlaylistTag} Upserted {Upserted} AlbumArtists",
                playlist.LoggingTag,
                albumArtists.Length
            );
        }
    }
}
