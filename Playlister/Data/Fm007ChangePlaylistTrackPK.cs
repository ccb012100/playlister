using FluentMigrator;

namespace Playlister.Data
{
    [Migration(007, description: "Alter the PlaylistTrack Primary Key to included added_at. This should give us (at least partial) support for duplicate tracks in a Playlist.")]
    public class Fm007ChangePlaylistTrackPK : Migration
    {
        public override void Up()
        {
            Execute.Script("Data/scripts/007_alter_playlist_track_pk.sql");
        }

        public override void Down() { }
    }
}
