using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Playlister.Models;

namespace Playlister.Repositories
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

            const string trackSql =
                @"INSERT INTO PlaylistTrack(id, name, track_number, disc_number, added_at, duration_ms, album_id, playlist_id, playlist_snapshot_id) VALUES(@Id, @Name, @TrackNumber, @DiscNumber, @AddedAt, @DurationMs, @AlbumId, @PlaylistId, @SnapshotId) " +
                // only update snapshot_id on conflict, because the rest should be the same
                "ON CONFLICT(id, playlist_id) DO UPDATE SET playlist_snapshot_id = excluded.playlist_snapshot_id " +
                "WHERE playlist_snapshot_id != excluded.playlist_snapshot_id;";

            await using SqliteConnection connection = _connectionFactory.Connection;
            await connection.OpenAsync(ct);
            DbTransaction txn = await connection.BeginTransactionAsync(ct);

            // we want to update all tracks because the snapshot_id needs to be updated
            await connection.ExecuteAsync(trackSql, items.Select(x => x.ToPlaylistTrack(playlist)), txn);

            const string artistSql = "INSERT INTO Artist(id, name) VALUES(@Id, @Name) ON CONFLICT(id) DO NOTHING;";

            ImmutableArray<string> existingTracks = await Get(items);
            // Get list of only new tracks so that we don't waste time trying to update stuff that already exists
            ImmutableArray<Track> newTracks =
                items.Where(i => !existingTracks.Contains(i.Track.Id)).Select(x => x.Track).ToImmutableArray();

            await connection.ExecuteAsync(artistSql, newTracks.Select(x => x.GetAllContainedArtists()), txn);

            const string albumSql =
                "INSERT INTO Album(id, name, total_tracks, album_type, release_date) VALUES(@Id, @Name, @TotalTracks, @AlbumType, @ReleaseDate) " +
                "ON CONFLICT(id) IGNORE;";

            ImmutableArray<Album> albums = newTracks.Select(t => t.Album).ToImmutableArray();

            await connection.ExecuteAsync(albumSql, albums, txn);

            const string albumArtistSql =
                "INSERT INTO AlbumArtist(album_id, artist_id) VALUES(@AlbumId, @ArtistId) " +
                "ON CONFLICT(album_id, artist_id) DO NOTHING";

            await connection.ExecuteAsync(albumArtistSql, albums.SelectMany(a => a.GetAlbumArtistPairings()), txn);

            const string trackArtistSql =
                "INSERT INTO TrackArtist(track_id, artist_id) VALUES(@TrackId, @ArtistId) " +
                "ON CONFLICT(track_id, artist_id) DO NOTHING";

            await connection.ExecuteAsync(trackArtistSql, newTracks.Select(x => x.GetArtistIdPairings()), txn);

            await txn.CommitAsync(ct);
        }

        private async Task<ImmutableArray<string>> Get(IEnumerable<PlaylistItem> playlistItems)
        {
            await using SqliteConnection connection = _connectionFactory.Connection;
            IEnumerable<PlaylistTrack> tracks =
                await connection.QueryAsync<PlaylistTrack>("SELECT * FROM PlaylistTrack WHERE id in @Ids",
                    playlistItems.Select(p => p.Track.Id));

            return tracks.Select(t => t.Id).ToImmutableArray();
        }
    }
}
