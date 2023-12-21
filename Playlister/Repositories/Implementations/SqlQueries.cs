namespace Playlister.Repositories.Implementations
{
    public static class SqlQueries
    {
        public static class Read
        {
            /// <summary>
            /// Returns a collection of <see cref="Models.Playlist"/>
            /// </summary>
            public const string Playlists = "SELECT * FROM Playlist";

            /// <summary>
            /// Returns a collection of (string, int) containing (PlaylistId, PlaylistTrack count)
            /// </summary>
            public const string PlaylistsWithMissingTracks =
@"select p.id, playlisttrack_count
from Playlist p
join (
    select playlist_id, count(*) as playlisttrack_count
    from PlaylistTrack
    group by playlist_id
) as pt on pt.playlist_id = p.id
where count_unique > playlisttrack_count";
        }

        public static class Upsert
        {
            public const string Album =
@"INSERT INTO Album(id, name, total_tracks, album_type, release_date)
VALUES(@Id, @Name, @TotalTracks, @AlbumType, @ReleaseDate)
ON CONFLICT(id)
DO UPDATE
SET name = excluded.name
WHERE name <> excluded.name;";

            public const string AlbumArtist =
@"INSERT INTO AlbumArtist(album_id, artist_id)
VALUES(@AlbumId, @ArtistId)
ON CONFLICT(album_id, artist_id)
DO NOTHING";

            public const string Artist =
@"INSERT INTO Artist(id, name)
VALUES(@Id, @Name)
ON CONFLICT(id)
DO UPDATE
SET name = excluded.name
WHERE name <> excluded.name;";

            /*
             * The snapshot is not updated when the details (name, description) change,
             * so we have to explicitly check for changes via the ON CONFLICT
             */
            public const string Playlist =
@"INSERT INTO Playlist(id, snapshot_id, name, collaborative, description, public, count, count_unique)
VALUES(@Id, @SnapshotId, @Name, @Collaborative, @Description, @Public, @Count, @CountUnique)
ON CONFLICT(id)
DO UPDATE
SET
    snapshot_id = excluded.snapshot_id,
    name = excluded.name,
    collaborative = excluded.collaborative,
    public = excluded.public,
    description = excluded.description,
    count = excluded.count,
    count_unique = excluded.count_unique
WHERE
    snapshot_id <> excluded.snapshot_id OR
    name <> excluded.name OR
    description <> excluded.description OR
    count <> excluded.count OR
    count_unique <> excluded.count_unique;";

            public const string PlaylistTrack =
@"INSERT INTO PlaylistTrack(track_id, playlist_id, playlist_snapshot_id, added_at)
VALUES(@TrackId, @PlaylistId, @SnapshotId, @AddedAt)
ON CONFLICT(track_id, playlist_id, added_at)
DO UPDATE
SET
    playlist_snapshot_id = excluded.playlist_snapshot_id,
    added_at = excluded.added_at
WHERE
    playlist_snapshot_id <> excluded.playlist_snapshot_id;";

            public const string Track =
@"INSERT INTO Track(id, name, track_number, disc_number, duration_ms, album_id)
VALUES(@Id, @Name, @TrackNumber, @DiscNumber, @DurationMs, @AlbumId)
ON CONFLICT(id)
DO UPDATE
SET
    name = excluded.name
WHERE
    name <> excluded.name;";

            public const string TrackArtist =
@"INSERT INTO TrackArtist(track_id, artist_id)
VALUES(@TrackId, @ArtistId)
ON CONFLICT(track_id, artist_id)
DO NOTHING;";
        }

        static public class Delete
        {
            /// <summary>
            /// Delete <see cref="Models.PlaylistTrack"/>s with outdated <see cref="Models.PlaylistTrack.SnapshotId"/>
            /// </summary>
            public const string OrphanedTracks =
@"DELETE from PlaylistTrack
WHERE (track_id, playlist_id, playlist_snapshot_id) IN (
    SELECT
        pt.track_id,
        pt.playlist_id,
        pt.playlist_snapshot_id
    FROM
        PlaylistTrack pt
    LEFT JOIN Playlist p ON pt.playlist_snapshot_id = p.snapshot_id
    WHERE
        p.id is null
);
SELECT changes();";
        }
    }
}
