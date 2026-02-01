using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Models;
using Playlister.Repositories;

namespace Playlister.Tests.Integration;

/// <summary>
///     Integration test that verifies playlist sync correctly adds new albums to a seeded database.
/// </summary>
public class PlaylistSyncNewAlbumIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>> {
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly SqliteConnection _db;

    public PlaylistSyncNewAlbumIntegrationTests( CustomWebApplicationFactory<Startup> factory ) {
        // Use seeded database
        factory.SeedDatabase = true;

        _factory = factory;
        _db = factory.Services.GetRequiredService<IConnectionFactory>( ).Connection;
    }

    [Fact]
    public async Task SyncPlaylistsAsync_WithNewAlbum_AddsOneAlbumToDatabase( ) {
        // Arrange
        using IServiceScope scope = _factory.Services.CreateScope( );
        IPlaylistWriteRepository writeRepo = scope.ServiceProvider.GetRequiredService<IPlaylistWriteRepository>( );

        // Get initial album count from seeded database
        int initialAlbumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Album" );

        // Create a playlist with a NEW album not in seed data
        Playlist testPlaylist = new( ) {
            Id = "playlist_new" ,
            SnapshotId = "snap_new" ,
            Name = "Test Playlist with New Album" ,
            Collaborative = false ,
            Description = "Contains one new album" ,
            Public = true ,
            Count = 2 ,
            CountUnique = 2
        };

        // Create playlist items with a NEW album not in seed data
        List<PlaylistItem> items = [
            CreatePlaylistItem(
                "track_new001" ,
                "New Track 1" ,
                "album_new001" ,
                "Brand New Album" ,
                "artist_new001" ,
                "New Test Artist" ,
                "2024-01-15" ,
                "2024-02-01T10:00:00Z" ,
                1 ,
                1 ,
                240000
            ) ,
            CreatePlaylistItem(
                "track_new002" ,
                "New Track 2" ,
                "album_new001" ,
                "Brand New Album" ,
                "artist_new001" ,
                "New Test Artist" ,
                "2024-01-15" ,
                "2024-02-01T10:01:00Z" ,
                2 ,
                1 ,
                260000
            )
        ];

        // Act - Sync the playlist with the new album using repository
        await writeRepo.UpsertAsync( testPlaylist , items , CancellationToken.None );

        // Rebuild PlaylistAlbum table
        await writeRepo.TruncateAndPopulatePlaylistAlbum( CancellationToken.None );

        // Assert - Verify exactly 1 new album was added
        int finalAlbumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Album" );

        Assert.Equal( initialAlbumCount + 1 , finalAlbumCount ); // should have added exactly 1 new album

        // Verify the new album exists in database
        Album? insertedAlbum = await _db.QuerySingleOrDefaultAsync<Album>(
            "SELECT * FROM Album WHERE id = @Id" ,
            new { Id = "album_new001" }
        );

        Assert.NotNull( insertedAlbum );
        Assert.Equal( "Brand New Album" , insertedAlbum.Name );

        // Verify the new artist exists
        Artist? insertedArtist = await _db.QuerySingleOrDefaultAsync<Artist>(
            "SELECT * FROM Artist WHERE id = @Id" ,
            new { Id = "artist_new001" }
        );

        Assert.NotNull( insertedArtist );
        Assert.Equal( "New Test Artist" , insertedArtist.Name );

        // Verify the new tracks exist
        int newTrackCount = await _db.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Track WHERE album_id = @AlbumId" ,
            new { AlbumId = "album_new001" }
        );

        Assert.Equal( 2 , newTrackCount ); // should have 2 tracks for the new album

        // Verify initial seeded album count was 10 (from seed_db.sql)
        Assert.Equal( 10 , initialAlbumCount );
    }

    private static PlaylistItem CreatePlaylistItem(
        string trackId ,
        string trackName ,
        string albumId ,
        string albumName ,
        string artistId ,
        string artistName ,
        string releaseDate ,
        string addedAt ,
        int trackNumber ,
        int discNumber ,
        int durationMs
    ) {
        Artist artist = new( ) { Id = artistId , Name = artistName };
        Album album = new( ) {
            Id = albumId ,
            Name = albumName ,
            TotalTracks = 2 ,
            AlbumType = "album" ,
            ReleaseDate = releaseDate ,
            ReleaseDatePrecision = "day" ,
            Artists = [artist]
        };

        Track track = new( ) {
            Id = trackId ,
            Name = trackName ,
            TrackNumber = trackNumber ,
            DiscNumber = discNumber ,
            DurationMs = durationMs ,
            Album = album ,
            Artists = [artist]
        };

        return new PlaylistItem {
            AddedAt = DateTime.Parse( addedAt ).ToUniversalTime( ) ,
            Track = track
        };
    }
}
