-- Seed data for Playlister integration tests
-- This script should be run AFTER create_db.sqlite

-- =============================================================================
-- Artists
-- =============================================================================
INSERT INTO Artist (id, name) VALUES
    ('artist001', 'The Beatles'),
    ('artist002', 'Pink Floyd'),
    ('artist003', 'Led Zeppelin'),
    ('artist004', 'Queen'),
    ('artist005', 'David Bowie'),
    ('artist006', 'Radiohead'),
    ('artist007', 'Nirvana'),
    ('artist008', 'Pearl Jam');

-- =============================================================================
-- Albums
-- =============================================================================
INSERT INTO Album (id, name, total_tracks, album_type, release_date) VALUES
    ('album001', 'Abbey Road', 17, 'album', '1969-09-26'),
    ('album002', 'The Dark Side of the Moon', 10, 'album', '1973-03-01'),
    ('album003', 'Led Zeppelin IV', 8, 'album', '1971-11-08'),
    ('album004', 'A Night at the Opera', 12, 'album', '1975-11-21'),
    ('album005', 'The Rise and Fall of Ziggy Stardust', 11, 'album', '1972-06-16'),
    ('album006', 'OK Computer', 12, 'album', '1997-05-21'),
    ('album007', 'Nevermind', 12, 'album', '1991-09-24'),
    ('album008', 'Ten', 11, 'album', '1991-08-27'),
    ('album009', 'In Rainbows', 10, 'album', '2007-10-10'),
    ('album010', 'MTV Unplugged in New York', 14, 'album', '1994-11-01');

-- =============================================================================
-- AlbumArtist (many-to-many relationship)
-- =============================================================================
INSERT INTO AlbumArtist (album_id, artist_id) VALUES
    ('album001', 'artist001'),
    ('album002', 'artist002'),
    ('album003', 'artist003'),
    ('album004', 'artist004'),
    ('album005', 'artist005'),
    ('album006', 'artist006'),
    ('album007', 'artist007'),
    ('album008', 'artist008'),
    ('album009', 'artist006'),
    ('album010', 'artist007');

-- =============================================================================
-- Tracks
-- =============================================================================
-- Abbey Road tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track001', 'Come Together', 1, 1, 259000, 'album001'),
    ('track002', 'Something', 2, 1, 183000, 'album001'),
    ('track003', 'Here Comes the Sun', 7, 1, 185000, 'album001');

-- The Dark Side of the Moon tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track004', 'Speak to Me', 1, 1, 68000, 'album002'),
    ('track005', 'Time', 4, 1, 413000, 'album002'),
    ('track006', 'Money', 6, 1, 382000, 'album002');

-- Led Zeppelin IV tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track007', 'Black Dog', 1, 1, 296000, 'album003'),
    ('track008', 'Stairway to Heaven', 4, 1, 482000, 'album003'),
    ('track009', 'Rock and Roll', 2, 1, 220000, 'album003');

-- A Night at the Opera tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track010', 'Bohemian Rhapsody', 11, 1, 355000, 'album004'),
    ('track011', 'You''re My Best Friend', 4, 1, 170000, 'album004');

-- Ziggy Stardust tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track012', 'Starman', 4, 1, 255000, 'album005'),
    ('track013', 'Ziggy Stardust', 9, 1, 194000, 'album005');

-- OK Computer tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track014', 'Paranoid Android', 2, 1, 386000, 'album006'),
    ('track015', 'Karma Police', 6, 1, 264000, 'album006'),
    ('track016', 'No Surprises', 10, 1, 228000, 'album006');

-- Nevermind tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track017', 'Smells Like Teen Spirit', 1, 1, 301000, 'album007'),
    ('track018', 'Come as You Are', 3, 1, 219000, 'album007'),
    ('track019', 'Lithium', 5, 1, 257000, 'album007');

-- Ten tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track020', 'Even Flow', 2, 1, 293000, 'album008'),
    ('track021', 'Alive', 4, 1, 341000, 'album008'),
    ('track022', 'Jeremy', 7, 1, 318000, 'album008');

-- In Rainbows tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track023', '15 Step', 1, 1, 237000, 'album009'),
    ('track024', 'Weird Fishes/Arpeggi', 4, 1, 318000, 'album009'),
    ('track025', 'Reckoner', 7, 1, 290000, 'album009');

-- MTV Unplugged tracks
INSERT INTO Track (id, name, track_number, disc_number, duration_ms, album_id) VALUES
    ('track026', 'About a Girl', 1, 1, 217000, 'album010'),
    ('track027', 'The Man Who Sold the World', 6, 1, 258000, 'album010');

-- =============================================================================
-- TrackArtist (many-to-many relationship)
-- =============================================================================
INSERT INTO TrackArtist (track_id, artist_id) VALUES
    ('track001', 'artist001'),
    ('track002', 'artist001'),
    ('track003', 'artist001'),
    ('track004', 'artist002'),
    ('track005', 'artist002'),
    ('track006', 'artist002'),
    ('track007', 'artist003'),
    ('track008', 'artist003'),
    ('track009', 'artist003'),
    ('track010', 'artist004'),
    ('track011', 'artist004'),
    ('track012', 'artist005'),
    ('track013', 'artist005'),
    ('track014', 'artist006'),
    ('track015', 'artist006'),
    ('track016', 'artist006'),
    ('track017', 'artist007'),
    ('track018', 'artist007'),
    ('track019', 'artist007'),
    ('track020', 'artist008'),
    ('track021', 'artist008'),
    ('track022', 'artist008'),
    ('track023', 'artist006'),
    ('track024', 'artist006'),
    ('track025', 'artist006'),
    ('track026', 'artist007'),
    ('track027', 'artist007');

-- =============================================================================
-- Playlists
-- =============================================================================
INSERT INTO Playlist (id, snapshot_id, name, collaborative, description, public, count, count_unique) VALUES
    ('playlist001', 'snap001', 'Classic Rock Essentials', 0, 'The best classic rock tracks', 1, 15, 15),
    ('playlist002', 'snap002', '90s Alternative', 0, 'Alternative rock from the 90s', 1, 9, 9),
    ('playlist003', 'snap003', 'British Invasion', 0, 'UK bands that changed music', 0, 8, 8),
    ('playlist004', 'snap004', '_queue Test Playlist', 0, 'Playlist with queue prefix for testing', 1, 3, 3),
    ('playlist005', 'snap005', 'Empty Playlist', 0, 'A playlist with no tracks', 1, 0, 0);

-- =============================================================================
-- PlaylistTrack (tracks in playlists with timestamps)
-- =============================================================================
-- Classic Rock Essentials playlist
INSERT INTO PlaylistTrack (track_id, playlist_id, playlist_snapshot_id, added_at) VALUES
    ('track001', 'playlist001', 'snap001', '2024-01-15 10:30:00'),
    ('track002', 'playlist001', 'snap001', '2024-01-15 10:31:00'),
    ('track003', 'playlist001', 'snap001', '2024-01-15 10:32:00'),
    ('track004', 'playlist001', 'snap001', '2024-01-15 10:33:00'),
    ('track005', 'playlist001', 'snap001', '2024-01-15 10:34:00'),
    ('track006', 'playlist001', 'snap001', '2024-01-15 10:35:00'),
    ('track007', 'playlist001', 'snap001', '2024-01-15 10:36:00'),
    ('track008', 'playlist001', 'snap001', '2024-01-15 10:37:00'),
    ('track010', 'playlist001', 'snap001', '2024-01-15 10:38:00'),
    ('track012', 'playlist001', 'snap001', '2024-01-15 10:39:00'),
    ('track013', 'playlist001', 'snap001', '2024-01-15 10:40:00'),
    ('track020', 'playlist001', 'snap001', '2024-01-15 10:41:00'),
    ('track021', 'playlist001', 'snap001', '2024-01-15 10:42:00'),
    ('track017', 'playlist001', 'snap001', '2024-01-15 10:43:00'),
    ('track018', 'playlist001', 'snap001', '2024-01-15 10:44:00');

-- 90s Alternative playlist
INSERT INTO PlaylistTrack (track_id, playlist_id, playlist_snapshot_id, added_at) VALUES
    ('track014', 'playlist002', 'snap002', '2024-02-01 14:00:00'),
    ('track015', 'playlist002', 'snap002', '2024-02-01 14:01:00'),
    ('track016', 'playlist002', 'snap002', '2024-02-01 14:02:00'),
    ('track017', 'playlist002', 'snap002', '2024-02-01 14:03:00'),
    ('track018', 'playlist002', 'snap002', '2024-02-01 14:04:00'),
    ('track019', 'playlist002', 'snap002', '2024-02-01 14:05:00'),
    ('track020', 'playlist002', 'snap002', '2024-02-01 14:06:00'),
    ('track021', 'playlist002', 'snap002', '2024-02-01 14:07:00'),
    ('track022', 'playlist002', 'snap002', '2024-02-01 14:08:00');

-- British Invasion playlist
INSERT INTO PlaylistTrack (track_id, playlist_id, playlist_snapshot_id, added_at) VALUES
    ('track001', 'playlist003', 'snap003', '2024-03-10 09:00:00'),
    ('track002', 'playlist003', 'snap003', '2024-03-10 09:01:00'),
    ('track003', 'playlist003', 'snap003', '2024-03-10 09:02:00'),
    ('track004', 'playlist003', 'snap003', '2024-03-10 09:03:00'),
    ('track005', 'playlist003', 'snap003', '2024-03-10 09:04:00'),
    ('track010', 'playlist003', 'snap003', '2024-03-10 09:05:00'),
    ('track012', 'playlist003', 'snap003', '2024-03-10 09:06:00'),
    ('track014', 'playlist003', 'snap003', '2024-03-10 09:07:00');

-- Queue test playlist (with PersistentQueueNamePrefix convention)
INSERT INTO PlaylistTrack (track_id, playlist_id, playlist_snapshot_id, added_at) VALUES
    ('track023', 'playlist004', 'snap004', '2024-04-01 12:00:00'),
    ('track024', 'playlist004', 'snap004', '2024-04-01 12:01:00'),
    ('track025', 'playlist004', 'snap004', '2024-04-01 12:02:00');

-- =============================================================================
-- PlaylistAlbum (denormalized view for searching)
-- =============================================================================
INSERT INTO PlaylistAlbum (artists, album, track_count, release_year, playlist, added_at, playlist_id, album_id) VALUES
    ('The Beatles', 'Abbey Road', 3, '1969', 'Classic Rock Essentials', '2024-01-15 10:30:00', 'playlist001', 'album001'),
    ('Pink Floyd', 'The Dark Side of the Moon', 3, '1973', 'Classic Rock Essentials', '2024-01-15 10:33:00', 'playlist001', 'album002'),
    ('Led Zeppelin', 'Led Zeppelin IV', 2, '1971', 'Classic Rock Essentials', '2024-01-15 10:36:00', 'playlist001', 'album003'),
    ('Queen', 'A Night at the Opera', 1, '1975', 'Classic Rock Essentials', '2024-01-15 10:38:00', 'playlist001', 'album004'),
    ('David Bowie', 'The Rise and Fall of Ziggy Stardust', 2, '1972', 'Classic Rock Essentials', '2024-01-15 10:39:00', 'playlist001', 'album005'),
    ('Pearl Jam', 'Ten', 2, '1991', 'Classic Rock Essentials', '2024-01-15 10:41:00', 'playlist001', 'album008'),
    ('Nirvana', 'Nevermind', 2, '1991', 'Classic Rock Essentials', '2024-01-15 10:43:00', 'playlist001', 'album007'),
    ('Radiohead', 'OK Computer', 3, '1997', '90s Alternative', '2024-02-01 14:00:00', 'playlist002', 'album006'),
    ('Nirvana', 'Nevermind', 3, '1991', '90s Alternative', '2024-02-01 14:03:00', 'playlist002', 'album007'),
    ('Pearl Jam', 'Ten', 3, '1991', '90s Alternative', '2024-02-01 14:06:00', 'playlist002', 'album008'),
    ('The Beatles', 'Abbey Road', 3, '1969', 'British Invasion', '2024-03-10 09:00:00', 'playlist003', 'album001'),
    ('Pink Floyd', 'The Dark Side of the Moon', 2, '1973', 'British Invasion', '2024-03-10 09:03:00', 'playlist003', 'album002'),
    ('Queen', 'A Night at the Opera', 1, '1975', 'British Invasion', '2024-03-10 09:05:00', 'playlist003', 'album004'),
    ('David Bowie', 'The Rise and Fall of Ziggy Stardust', 1, '1972', 'British Invasion', '2024-03-10 09:06:00', 'playlist003', 'album005'),
    ('Radiohead', 'OK Computer', 1, '1997', 'British Invasion', '2024-03-10 09:07:00', 'playlist003', 'album006'),
    ('Radiohead', 'In Rainbows', 3, '2007', '_queue Test Playlist', '2024-04-01 12:00:00', 'playlist004', 'album009');

-- =============================================================================
-- SavedAlbum (albums in user's library)
-- =============================================================================
INSERT INTO SavedAlbum (id, label) VALUES
    ('album001', 'Apple Records'),
    ('album002', 'Harvest'),
    ('album006', 'Parlophone');

-- =============================================================================
-- ExternalId (external identifiers for saved albums)
-- =============================================================================
INSERT INTO ExternalId (album_id, upc) VALUES
    ('album001', '077774664424'),
    ('album002', '077774831420'),
    ('album006', '724385522925');
