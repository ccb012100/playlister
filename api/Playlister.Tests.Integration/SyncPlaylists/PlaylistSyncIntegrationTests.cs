using System.Reflection;

using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Models;
using Playlister.RefitClients;
using Playlister.Repositories;
using Playlister.Services;
using Playlister.Services.Implementations;
using Playlister.Tests.Integration.Mocks;

namespace Playlister.Tests.Integration.SyncPlaylists;

/// <summary>
///     Integration test that verifies playlist sync functionality using PlaylistService
///     with mock Spotify API data, testing the service and repository layers together.
/// </summary>
public class PlaylistSyncIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>> {
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly SqliteConnection _db;

    public PlaylistSyncIntegrationTests( CustomWebApplicationFactory<Startup> factory ) {
        // Don't seed the database for this test - we want to test syncing into an empty database
        factory.SeedDatabase = false;
        _factory = factory;
        _db = factory.Services.GetRequiredService<IConnectionFactory>( ).Connection;

        // Clear the static caches in PlaylistService to ensure test isolation
        // This is necessary because the service uses static caches that persist across test instances
        ClearPlaylistServiceCaches( );
    }

    /// <summary>
    ///     Clears the static caches used by PlaylistService to ensure test isolation.
    ///     The service uses static CacheObject instances that persist across service instances,
    ///     so we need to reset them before each test.
    /// </summary>
    private static void ClearPlaylistServiceCaches( ) {
        var serviceType = typeof( PlaylistService );

        // Clear s_playlistCache
        var playlistCacheField = serviceType.GetField( "s_playlistCache" , BindingFlags.NonPublic | BindingFlags.Static );
        if ( playlistCacheField?.GetValue( null ) is { } playlistCache ) {
            var clearMethod = playlistCache.GetType( ).GetMethod( "Clear" , BindingFlags.Public | BindingFlags.Instance );
            clearMethod?.Invoke( playlistCache , null );
        }

        // Clear s_missingTracksCache
        var missingTracksCacheField = serviceType.GetField( "s_missingTracksCache" , BindingFlags.NonPublic | BindingFlags.Static );
        if ( missingTracksCacheField?.GetValue( null ) is { } missingTracksCache ) {
            var clearMethod = missingTracksCache.GetType( ).GetMethod( "Clear" , BindingFlags.Public | BindingFlags.Instance );
            clearMethod?.Invoke( missingTracksCache , null );
        }

        // Clear s_updatedPlaylistsCache
        var updatedPlaylistsCacheField = serviceType.GetField( "s_updatedPlaylistsCache" , BindingFlags.NonPublic | BindingFlags.Static );
        if ( updatedPlaylistsCacheField?.GetValue( null ) is { } updatedPlaylistsCache ) {
            var clearMethod = updatedPlaylistsCache.GetType( ).GetMethod( "Clear" , BindingFlags.Public | BindingFlags.Instance );
            clearMethod?.Invoke( updatedPlaylistsCache , null );
        }
    }

    [Fact]
    public async Task SyncPlaylistsAsync_SyncsAllPlaylistsWithoutQueuePrefix_WhenCalledWithAccessToken( ) {
        // ARRANGE
        const string accessToken = "test-token";
        var playlistService = _factory.Services.GetRequiredService<IPlaylistService>( );
        var mockApi = (MockSpotifyApiProvider)_factory.Services.GetRequiredService<ISpotifyApi>( );

        // Verify database is empty before test
        int initialPlaylistCount = _db.QuerySingleOrDefault<int>( "SELECT COUNT(*) FROM Playlist" );
        initialPlaylistCount.Should( ).Be( 0 );

        // Get playlists from mock API and filter out queue playlists
        var playlistResponse = await mockApi.GetCurrentUserPlaylistsAsync( $"Bearer {accessToken}" , null , null , CancellationToken.None );
        var playlistsToSync = playlistResponse.Items
            .Where( p => !p.Name.Contains( "_queue" ) )
            .Select( p => p.Id )
            .ToList( );

        playlistsToSync.Should( ).HaveCount( 4 );

        // ACT - Use ForceSyncPlaylistAsync to bypass the service's static cache checks
        // and directly sync each playlist with fresh data from the mock API
        foreach ( var playlistId in playlistsToSync ) {
            await playlistService.ForceSyncPlaylistAsync( accessToken , playlistId , CancellationToken.None );
        }

        // ASSERT - Verify all Spotify API data has been synced to database
        // 4 playlists synced (excluding the 2 _queue and empty playlists)
        int syncedPlaylistCount = _db.QuerySingleOrDefault<int>( "SELECT COUNT(*) FROM Playlist WHERE Name NOT LIKE '%_queue%'" );
        syncedPlaylistCount.Should( ).Be( 4 );

        // All artists from non-queue mock data should be synced (8 artists)
        int syncedArtistCount = _db.QuerySingleOrDefault<int>( "SELECT COUNT(*) FROM Artist" );
        syncedArtistCount.Should( ).Be( 8 );

        // Albums from non-queue playlists: album001-008 (album009-010 are only in queue/empty)
        int syncedAlbumCount = _db.QuerySingleOrDefault<int>( "SELECT COUNT(*) FROM Album" );
        syncedAlbumCount.Should( ).Be( 8 );

        // Tracks from non-queue playlists: track001-008, 010, 012-022 (excluding 009, 011, 023-027)
        int syncedTrackCount = _db.QuerySingleOrDefault<int>( "SELECT COUNT(*) FROM Track" );
        syncedTrackCount.Should( ).Be( 20 );
    }
}
