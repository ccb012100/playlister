using Dapper;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Repositories;

using Xunit.Abstractions;

namespace Playlister.Tests.Integration;

public class PlaylisterTests( CustomWebApplicationFactory<Startup> webApplicationFactory )
    : IClassFixture<CustomWebApplicationFactory<Startup>> {
    private readonly HttpClient _client = webApplicationFactory.CreateClient( new WebApplicationFactoryClientOptions { AllowAutoRedirect = false } );
    private readonly SqliteConnection _db = webApplicationFactory.Services.GetService<IConnectionFactory>( )!.Connection;

    [Fact]
    public async Task CreatesDatabaseOnStartup( ) {
        // arrange
        string[ ] indexes = [
            "IX_Album_release_date" ,
            "IX_ExternalId_album_id" ,
            "IX_PlaylistAlbum_album_artists" ,
            "IX_PlaylistAlbum_album_id" ,
            "IX_PlaylistAlbum_artists_album" ,
            "IX_PlaylistAlbum_id" ,
            "IX_PlaylistAlbum_playlist_artists_album" ,
            "IX_PlaylistAlbum_playlist_id" ,
            "IX_PlaylistAlbum_release_year_artists_album" ,
            "IX_PlaylistAlbum_release_year" ,
            "IX_PlaylistTrack_added_at" ,
            "IX_PlaylistTrack_album_id" ,
            "IX_PlaylistTrack_playlist_id" ,
            "IX_PlaylistTrack_playlist_snapshot_id" ,
            "IX_SavedAlbum_id" ,
            "IX_Playlist_snapshot_id" ,
            "UC_Version"
        ];

        string[ ] tables = [
            "Album" ,
            "AlbumArtist" ,
            "Artist" ,
            "ExternalId" ,
            "Playlist" ,
            "PlaylistAlbum" ,
            "PlaylistTrack" ,
            "SavedAlbum" ,
            "Track" ,
            "TrackArtist" ,
            "VersionInfo"
        ];

        string[ ] triggers = [
            "album_artist_modified" ,
            "album_modified" ,
            "artist_modified" ,
            "playlist_album_modified" ,
            "playlist_modified" ,
            "playlist_track_modified" ,
            "track_artist_modified" ,
            "track_modified"
        ];

        // act
        SqliteSchema[ ] schema = ( await _db.QueryAsync<SqliteSchema>( "select * from sqlite_schema" ) ).ToArray( );

        // assert
        schema.Should( ).NotBeEmpty( );

        indexes.Should( )
            .AllSatisfy(
                idx => { schema.Should( ).ContainSingle( row => row.Type == "index" && row.Name == idx ); }
            );

        tables.Should( )
            .AllSatisfy(
                table => { schema.Should( ).ContainSingle( row => row.Type == "table" && row.Name == table ); }
            );

        triggers.Should( )
            .AllSatisfy(
                trigger => { schema.Should( ).ContainSingle( row => row.Type == "trigger" && row.Name == trigger ); }
            );
    }
}
