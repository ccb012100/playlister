create trigger playlist_modified
    after update
    on Playlist
begin
    update Playlist set modified_at = current_timestamp where id = new.id;
end;

create trigger artist_modified
    after update
    on Artist
begin
    update Artist set modified_at = current_timestamp where id = new.id;
end;

create trigger album_modified
    after update
    on Album
begin
    update Album set modified_at = current_timestamp where id = new.id;
end;

create trigger playlist_track_modified
    after update
    on PlaylistTrack
begin
    update PlaylistTrack set modified_at = current_timestamp where id = new.id;
end;

create trigger track_modified
    after update
    on Track
begin
    update Track set modified_at = current_timestamp where id = new.id;
end;
