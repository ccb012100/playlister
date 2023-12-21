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

CREATE INDEX "IX_PlaylistTrack_added_at" ON "PlaylistTrack"("added_at" DESC);
CREATE INDEX "IX_PlaylistTrack_album_id" ON "PlaylistTrack"("album_id" ASC);
CREATE INDEX "IX_PlaylistTrack_playlist_id" ON "PlaylistTrack"(
  "playlist_id" ASC
);
CREATE INDEX "IX_PlaylistTrack_playlist_snapshot_id" ON "PlaylistTrack"(
  "playlist_snapshot_id" ASC
);

CREATE TRIGGER playlist_track_modified
    after update
    on PlaylistTrack
begin
    update PlaylistTrack
    set modified_at = current_timestamp
    where track_id = new.track_id
      and playlist_id = new.playlist_id;
end;

INSERT INTO PlaylistTrack SELECT * FROM PlaylistTrack_old;

DROP TABLE PlaylistTrack_old;

PRAGMA foreign_keys=on;
