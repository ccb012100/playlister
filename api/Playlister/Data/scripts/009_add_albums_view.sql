CREATE VIEW AlbumsView
AS
    SELECT GROUP_CONCAT(artist, '; ') AS artists, album, track_count, release_date, added_at, playlist
    FROM
        (
            SELECT
                art.name AS artist,
                a.name AS album,
                a.id AS album_id,
                a.total_tracks AS track_count,
                substr(a.release_date, 1, 4) AS release_date,
                pt.added_at,
                p.name AS playlist,
                p.id AS playlist_id
            FROM Album a
                JOIN albumartist aa ON aa.album_id = a.id
                JOIN artist art ON art.id = aa.artist_id
                JOIN track t ON t.album_id = a.id
                JOIN playlisttrack pt ON pt.track_id = t.id
                JOIN playlist p ON p.id = pt.playlist_id
            GROUP BY a.id, art.id, p.id
            ORDER BY p.id, a.id, art.name
        )
    GROUP BY album_id, playlist_id
    ORDER BY added_at DESC;
