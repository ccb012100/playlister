create trigger playlist_modified
    after update
    on Playlist
begin
    update Playlist set modified_at = current_timestamp where id = new.id;
end;
