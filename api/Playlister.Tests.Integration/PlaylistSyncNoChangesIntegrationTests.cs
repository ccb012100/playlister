using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Models;
using Playlister.Repositories;
using Playlister.Services;

namespace Playlister.Tests.Integration;

/// <summary>
///     Integration test that verifies syncing playlists with unchanged data results in no database updates.
/// </summary>
public class PlaylistSyncNoChangesIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>> {
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly SqliteConnection _db;

    public PlaylistSyncNoChangesIntegrationTests( CustomWebApplicationFactory<Startup> factory ) {
        // Configure factory to seed the database with test data
        factory.SeedDatabase = true;

        _factory = factory;
        _db = factory.Services.GetRequiredService<IConnectionFactory>( ).Connection;
    }

    [Fact]
    public async Task SyncPlaylistsAsync_WithUnchangedData_ResultsInNoUpdates( ) {
        // Arrange - Get initial database state
        int initialArtistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Artist" );
        int initialAlbumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Album" );
        int initialTrackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Track" );
        int initialPlaylistTrackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM PlaylistTrack" );
        DateTime? initialPlaylistModified = await _db.ExecuteScalarAsync<DateTime?>( "SELECT modified_at FROM Playlist WHERE id = 'playlist001'" );

        using IServiceScope scope = _factory.Services.CreateScope( );
        IPlaylistService playlistService = scope.ServiceProvider.GetRequiredService<IPlaylistService>( );

        // Create playlists matching the seeded database exactly (same snapshot IDs)
        List<Playlist> playlistsToSync =
        [
            new Playlist {
                Id = "playlist001" ,
                SnapshotId = "snap001" , // Same as seed_db.sql
                Name = "Classic Rock Essentials" ,
                Collaborative = false ,
                Description = "The best classic rock tracks" ,
                Public = true ,
                Count = 15 ,
                CountUnique = 15
            } ,
            new Playlist {
                Id = "playlist002" ,
                SnapshotId = "snap002" , // Same as seed_db.sql
                Name = "90s Alternative" ,
                Collaborative = false ,
                Description = "Alternative rock from the 90s" ,
                Public = true ,
                Count = 9 ,
                CountUnique = 9
            }
        ];

        // Act - Sync playlists with same data as database
        int updatedCount = await playlistService.SyncPlaylistsAsync( "fake-token" , playlistsToSync , CancellationToken.None );

        // Assert - No updates should have occurred
        updatedCount.Should( ).Be( 0 , "no playlists should be updated when data is unchanged" );

        // Verify database counts remain the same
        int finalArtistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Artist" );
        int finalAlbumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Album" );
        int finalTrackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Track" );
        int finalPlaylistTrackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM PlaylistTrack" );
        DateTime? finalPlaylistModified = await _db.ExecuteScalarAsync<DateTime?>( "SELECT modified_at FROM Playlist WHERE id = 'playlist001'" );

        finalArtistCount.Should( ).Be( initialArtistCount , "artist count should not change" );
        finalAlbumCount.Should( ).Be( initialAlbumCount , "album count should not change" );
        finalTrackCount.Should( ).Be( initialTrackCount , "track count should not change" );
        finalPlaylistTrackCount.Should( ).Be( initialPlaylistTrackCount , "playlist track count should not change" );
        finalPlaylistModified.Should( ).Be( initialPlaylistModified , "playlist modified_at timestamp should not change when data is unchanged" );

        // Verify the seeded data is as expected
        initialArtistCount.Should( ).Be( 8 , "seed data should have 8 artists" );
        initialAlbumCount.Should( ).Be( 10 , "seed data should have 10 albums" );
        initialTrackCount.Should( ).Be( 27 , "seed data should have 27 tracks" );
        initialPlaylistTrackCount.Should( ).Be( 35 , "seed data should have 35 playlist track associations" );
    }
}
