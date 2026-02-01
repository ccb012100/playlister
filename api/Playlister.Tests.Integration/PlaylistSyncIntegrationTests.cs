using Dapper;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Models;
using Playlister.Repositories;

namespace Playlister.Tests.Integration;

/// <summary>
///     Integration test that verifies playlist sync functionality by starting with an empty database
///     and syncing playlists through the actual application logic.
/// </summary>
public class PlaylistSyncIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>> {
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly SqliteConnection _db;

    public PlaylistSyncIntegrationTests( CustomWebApplicationFactory<Startup> factory ) {
        _factory = factory;
        _db = factory.Services.GetRequiredService<IConnectionFactory>( ).Connection;
    }

    [Fact]
    public async Task SyncPlaylistsAsync_WithEmptyDatabase_PopulatesDataCorrectly( ) {
        // Arrange
        using IServiceScope scope = _factory.Services.CreateScope( );
        IPlaylistWriteRepository writeRepo = scope.ServiceProvider.GetRequiredService<IPlaylistWriteRepository>( );

        // Create test playlists matching seed_db.sql
        Playlist classicRock = new( ) {
            Id = "playlist001" ,
            SnapshotId = "snap001" ,
            Name = "Classic Rock Essentials" ,
            Collaborative = false ,
            Description = "The best classic rock tracks" ,
            Public = true ,
            Count = 15 ,
            CountUnique = 15
        };

        Playlist nineties = new( ) {
            Id = "playlist002" ,
            SnapshotId = "snap002" ,
            Name = "90s Alternative" ,
            Collaborative = false ,
            Description = "Alternative rock from the 90s" ,
            Public = true ,
            Count = 9 ,
            CountUnique = 9
        };

        // Create playlist items (tracks) for Classic Rock playlist
        List<PlaylistItem> classicRockItems = CreateClassicRockPlaylistItems( );
        List<PlaylistItem> ninetiesItems = CreateNinetiesPlaylistItems( );

        // Act - Sync playlists through repository (simulating what the service would do)
        await writeRepo.UpsertAsync( classicRock , classicRockItems , CancellationToken.None );
        await writeRepo.UpsertAsync( nineties , ninetiesItems , CancellationToken.None );

        // Rebuild PlaylistAlbum table
        await writeRepo.TruncateAndPopulatePlaylistAlbum( CancellationToken.None );

        // Assert - Verify database state matches seed expectations
        int artistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Artist" );
        int albumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Album" );
        int trackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Track" );
        int playlistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Playlist" );
        int playlistTrackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM PlaylistTrack" );

        // Both playlists share some albums/tracks, so counts will be less than the sum
        artistCount.Should( ).Be( 8 , "should have synced 8 unique artists" );
        albumCount.Should( ).Be( 8 , "should have synced 8 unique albums (Beatles, Pink Floyd, Led Zeppelin, Queen, Bowie, Pearl Jam, Nirvana, Radiohead)" );
        trackCount.Should( ).Be( 20 , "should have synced 20 unique tracks" );
        playlistCount.Should( ).Be( 2 , "should have synced 2 playlists" );
        playlistTrackCount.Should( ).Be( 24 , "should have 24 total playlist-track associations (15+9)" );

        // Verify PlaylistAlbum was populated
        int playlistAlbumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM PlaylistAlbum" );
        playlistAlbumCount.Should( ).BeGreaterThan( 0 , "PlaylistAlbum table should be populated" );
    }

    private static List<PlaylistItem> CreateClassicRockPlaylistItems( ) {
        return
        [
            CreatePlaylistItem( "track001" , "Come Together" , "album001" , "Abbey Road" , "artist001" , "The Beatles" , "1969-09-26" , "2024-01-15T10:30:00Z" , 1 , 1 , 259000 ) ,
            CreatePlaylistItem( "track002" , "Something" , "album001" , "Abbey Road" , "artist001" , "The Beatles" , "1969-09-26" , "2024-01-15T10:31:00Z" , 2 , 1 , 183000 ) ,
            CreatePlaylistItem( "track003" , "Here Comes the Sun" , "album001" , "Abbey Road" , "artist001" , "The Beatles" , "1969-09-26" , "2024-01-15T10:32:00Z" , 7 , 1 , 185000 ) ,
            CreatePlaylistItem( "track004" , "Speak to Me" , "album002" , "The Dark Side of the Moon" , "artist002" , "Pink Floyd" , "1973-03-01" , "2024-01-15T10:33:00Z" , 1 , 1 , 68000 ) ,
            CreatePlaylistItem( "track005" , "Time" , "album002" , "The Dark Side of the Moon" , "artist002" , "Pink Floyd" , "1973-03-01" , "2024-01-15T10:34:00Z" , 4 , 1 , 413000 ) ,
            CreatePlaylistItem( "track006" , "Money" , "album002" , "The Dark Side of the Moon" , "artist002" , "Pink Floyd" , "1973-03-01" , "2024-01-15T10:35:00Z" , 6 , 1 , 382000 ) ,
            CreatePlaylistItem( "track007" , "Black Dog" , "album003" , "Led Zeppelin IV" , "artist003" , "Led Zeppelin" , "1971-11-08" , "2024-01-15T10:36:00Z" , 1 , 1 , 296000 ) ,
            CreatePlaylistItem( "track008" , "Stairway to Heaven" , "album003" , "Led Zeppelin IV" , "artist003" , "Led Zeppelin" , "1971-11-08" , "2024-01-15T10:37:00Z" , 4 , 1 , 482000 ) ,
            CreatePlaylistItem( "track010" , "Bohemian Rhapsody" , "album004" , "A Night at the Opera" , "artist004" , "Queen" , "1975-11-21" , "2024-01-15T10:38:00Z" , 11 , 1 , 355000 ) ,
            CreatePlaylistItem( "track012" , "Starman" , "album005" , "The Rise and Fall of Ziggy Stardust" , "artist005" , "David Bowie" , "1972-06-16" , "2024-01-15T10:39:00Z" , 4 , 1 , 255000 ) ,
            CreatePlaylistItem( "track013" , "Ziggy Stardust" , "album005" , "The Rise and Fall of Ziggy Stardust" , "artist005" , "David Bowie" , "1972-06-16" , "2024-01-15T10:40:00Z" , 9 , 1 , 194000 ) ,
            CreatePlaylistItem( "track020" , "Even Flow" , "album008" , "Ten" , "artist008" , "Pearl Jam" , "1991-08-27" , "2024-01-15T10:41:00Z" , 2 , 1 , 293000 ) ,
            CreatePlaylistItem( "track021" , "Alive" , "album008" , "Ten" , "artist008" , "Pearl Jam" , "1991-08-27" , "2024-01-15T10:42:00Z" , 4 , 1 , 341000 ) ,
            CreatePlaylistItem( "track017" , "Smells Like Teen Spirit" , "album007" , "Nevermind" , "artist007" , "Nirvana" , "1991-09-24" , "2024-01-15T10:43:00Z" , 1 , 1 , 301000 ) ,
            CreatePlaylistItem( "track018" , "Come as You Are" , "album007" , "Nevermind" , "artist007" , "Nirvana" , "1991-09-24" , "2024-01-15T10:44:00Z" , 3 , 1 , 219000 )
        ];
    }

    private static List<PlaylistItem> CreateNinetiesPlaylistItems( ) {
        return
        [
            CreatePlaylistItem( "track014" , "Paranoid Android" , "album006" , "OK Computer" , "artist006" , "Radiohead" , "1997-05-21" , "2024-02-01T14:00:00Z" , 2 , 1 , 386000 ) ,
            CreatePlaylistItem( "track015" , "Karma Police" , "album006" , "OK Computer" , "artist006" , "Radiohead" , "1997-05-21" , "2024-02-01T14:01:00Z" , 6 , 1 , 264000 ) ,
            CreatePlaylistItem( "track016" , "No Surprises" , "album006" , "OK Computer" , "artist006" , "Radiohead" , "1997-05-21" , "2024-02-01T14:02:00Z" , 10 , 1 , 228000 ) ,
            CreatePlaylistItem( "track017" , "Smells Like Teen Spirit" , "album007" , "Nevermind" , "artist007" , "Nirvana" , "1991-09-24" , "2024-02-01T14:03:00Z" , 1 , 1 , 301000 ) ,
            CreatePlaylistItem( "track018" , "Come as You Are" , "album007" , "Nevermind" , "artist007" , "Nirvana" , "1991-09-24" , "2024-02-01T14:04:00Z" , 3 , 1 , 219000 ) ,
            CreatePlaylistItem( "track019" , "Lithium" , "album007" , "Nevermind" , "artist007" , "Nirvana" , "1991-09-24" , "2024-02-01T14:05:00Z" , 5 , 1 , 257000 ) ,
            CreatePlaylistItem( "track020" , "Even Flow" , "album008" , "Ten" , "artist008" , "Pearl Jam" , "1991-08-27" , "2024-02-01T14:06:00Z" , 2 , 1 , 293000 ) ,
            CreatePlaylistItem( "track021" , "Alive" , "album008" , "Ten" , "artist008" , "Pearl Jam" , "1991-08-27" , "2024-02-01T14:07:00Z" , 4 , 1 , 341000 ) ,
            CreatePlaylistItem( "track022" , "Jeremy" , "album008" , "Ten" , "artist008" , "Pearl Jam" , "1991-08-27" , "2024-02-01T14:08:00Z" , 7 , 1 , 318000 )
        ];
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
            TotalTracks = 10 ,
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
