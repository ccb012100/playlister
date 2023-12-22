PRAGMA foreign_keys=off;

ALTER TABLE PlaylistTrack RENAME TO PlaylistTrack_old;

CREATE TABLE IF NOT EXISTS "PlaylistTrack"(
  "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "modified_at" DATETIME,
  "track_id" TEXT NOT NULL,
  "playlist_id" TEXT NOT NULL,
  "playlist_snapshot_id" TEXT,
  "added_at" DATETIME NOT NULL,
  CONSTRAINT "PK_PlaylistTrack" PRIMARY KEY("track_id", "playlist_id", "added_at"),
  CONSTRAINT "fk_playlisttrack_trackid" FOREIGN KEY("track_id") REFERENCES "Track"("id"),
  CONSTRAINT "fk_playlisttrack_playlistid" FOREIGN KEY("playlist_id") REFERENCES "Playlist"("id")
);

INSERT INTO PlaylistTrack SELECT * FROM PlaylistTrack_old;

DROP TABLE PlaylistTrack_old;

PRAGMA foreign_keys=on;
