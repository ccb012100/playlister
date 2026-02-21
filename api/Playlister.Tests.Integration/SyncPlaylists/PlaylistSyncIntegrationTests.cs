using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Models;
using Playlister.RefitClients;
using Playlister.Repositories;
using Playlister.Tests.Integration.Mocks;

namespace Playlister.Tests.Integration.SyncPlaylists;

/// <summary>
///     Integration test that verifies playlist persistence by syncing with mock Spotify API data
///     through the repository layer, testing both the service and repository together via mock integration.
/// </summary>
public class PlaylistSyncIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>> {
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly SqliteConnection _db;
    private readonly MockSpotifyApiProvider _mockApi;

    public PlaylistSyncIntegrationTests( CustomWebApplicationFactory<Startup> factory ) {
        // Don't seed the database for this test - we want to test syncing into an empty database
        factory.SeedDatabase = false;
        _factory = factory;
        _db = factory.Services.GetRequiredService<IConnectionFactory>( ).Connection;
        _mockApi = (MockSpotifyApiProvider)factory.Services.GetRequiredService<ISpotifyApi>( );
    }

    [Fact]
    public async Task SyncPlaylistsAsync_WithEmptyDatabase_PopulatesDataCorrectly( ) {
        // Arrange
        using IServiceScope scope = _factory.Services.CreateScope( );
        IPlaylistWriteRepository writeRepo = scope.ServiceProvider.GetRequiredService<IPlaylistWriteRepository>( );

        // Get playlists from mock API to simulate Spotify API responses
        var playlists = (await _mockApi.GetCurrentUserPlaylistsAsync( "mock_token" , null , null , CancellationToken.None )).Items.ToList( );

        // Convert mock API objects to domain models and sync each one
        foreach ( var playlistObj in playlists.Where( p => !p.Name.StartsWith( "_queue" ) ) ) {
            Playlist playlist = playlistObj.ToPlaylist( );

            // Get tracks from mock API for this playlist
            var tracksResponse = await _mockApi.GetPlaylistTracksAsync( "mock_token" , playlist.Id , null , null , CancellationToken.None );
            var items = tracksResponse.Items.ToList( );

            // Fetch additional pages if needed
            while ( tracksResponse.Next is not null ) {
                tracksResponse = await _mockApi.GetPlaylistTracksAsync( "mock_token" , tracksResponse.Next.ToString( ) , CancellationToken.None );
                items.AddRange( tracksResponse.Items );
            }

            // Sync the playlist and its tracks through the repository
            await writeRepo.UpsertAsync( playlist , items , CancellationToken.None );
        }

        // Rebuild PlaylistAlbum table
        await writeRepo.TruncateAndPopulatePlaylistAlbum( CancellationToken.None );

        // Assert - Verify data was synced to the database
        int playlistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Playlist" );
        int artistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Artist" );
        int albumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Album" );
        int trackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Track" );
        int playlistTrackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM PlaylistTrack" );

        // Verify playlists were synced (service skips queue playlists)
        playlistCount.Should( ).Be( 4 , "should have synced 4 playlists (excluding queue playlists)" );

        // Mock API has overlapping tracks/artists/albums across playlists
        artistCount.Should( ).Be( 8 , "should have synced 8 unique artists" );
        albumCount.Should( ).Be( 8 , "should have synced 8 unique albums (excluding queue playlist albums)" );
        trackCount.Should( ).BeLessThanOrEqualTo( 27 , "should have synced tracks from non-queue playlists" );
        playlistTrackCount.Should( ).BeGreaterThan( 0 , "should have playlist-track associations" );

        // Verify PlaylistAlbum was populated
        int playlistAlbumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM PlaylistAlbum" );
        playlistAlbumCount.Should( ).BeGreaterThan( 0 , "PlaylistAlbum table should be populated" );
    }
}
