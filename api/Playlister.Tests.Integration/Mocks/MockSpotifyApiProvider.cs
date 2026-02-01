using System.Web;

using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Models.SpotifyApi.Enums;
using Playlister.RefitClients;

namespace Playlister.Tests.Integration.Mocks;

/// <summary>
///     Mock implementation of ISpotifyApi that returns test data corresponding to seed_db.sql.
///     Provides 5 playlists with 8 albums and 27 tracks across 8 artists.
/// </summary>
public class MockSpotifyApiProvider : ISpotifyApi {
    private static readonly Uri s_baseUri = new( "https://api.spotify.com/v1" );
    private static readonly Uri s_spotifyUri = new( "https://open.spotify.com" );

    private readonly Dictionary<string, SimplifiedPlaylistObject> _playlists;
    private readonly Dictionary<string, PagingObject<Models.PlaylistItem>> _playlistTracks;
    private readonly Dictionary<string, Artist> _artists;
    private readonly Dictionary<string, Album> _albums;
    private readonly Dictionary<string, Track> _tracks;

    public MockSpotifyApiProvider( ) {
        _playlists = new( );
        _playlistTracks = new( );
        _artists = new( );
        _albums = new( );
        _tracks = new( );
        InitializeTestData( );
    }

    public Task<PrivateUserObject> GetCurrentUserAsync( string token , CancellationToken ct ) {
        var user = new PrivateUserObject {
            Country = "US" ,
            DisplayName = "Test User" ,
            Email = "test@example.com" ,
            ExternalUrls = new ExternalUrlObject { Spotify = new Uri( $"{s_spotifyUri}/user/testuser" ) } ,
            Followers = new FollowersObject { Total = 100 } ,
            Images = new List<ImageObject> {
                new ImageObject { Height = 300 , Width = 300 , Url = new Uri( "https://example.com/image.jpg" ) }
            } ,
            Product = "premium" ,
            Href = new Uri( $"{s_baseUri}/users/testuser" ) ,
            Id = "testuser" ,
            Type = ObjectType.User ,
            Uri = new Uri( "spotify:user:testuser" )
        };

        return Task.FromResult( user );
    }

    public Task<SimplifiedPlaylistObject> GetPlaylistAsync( string token , string playlistId , CancellationToken ct ) {
        if ( !_playlists.TryGetValue( playlistId , out var playlist ) ) {
            throw new InvalidOperationException( $"Playlist {playlistId} not found" );
        }

        return Task.FromResult( playlist );
    }

    public Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync(
        string token ,
        int? offset ,
        int? limit ,
        CancellationToken ct
    ) {
        offset ??= 0;
        limit ??= 50;

        var playlists = _playlists.Values.ToList( );
        var paging = new PagingObject<SimplifiedPlaylistObject> {
            Href = new Uri( $"{s_baseUri}/me/playlists" ) ,
            Items = playlists.Skip( offset.Value ).Take( limit.Value ) ,
            Limit = limit.Value ,
            Next = offset.Value + limit.Value < playlists.Count ? new Uri( $"{s_baseUri}/me/playlists?offset={offset.Value + limit.Value}&limit={limit.Value}" ) : null ,
            Offset = offset.Value ,
            Previous = offset.Value > 0 ? new Uri( $"{s_baseUri}/me/playlists?offset={Math.Max( 0 , offset.Value - limit.Value )}&limit={limit.Value}" ) : null ,
            Total = playlists.Count
        };

        return Task.FromResult( paging );
    }

    public Task<PagingObject<SavedAlbumObject>> GetCurrentUserSavedAlbums( string token , int? offset , int? limit , CancellationToken ct ) {
        throw new NotImplementedException( );
    }

    public Task<PagingObject<Models.PlaylistItem>> GetPlaylistTracksAsync(
        string token ,
        string playlistId ,
        int? offset ,
        int? limit ,
        CancellationToken ct
    ) {
        if ( !_playlistTracks.TryGetValue( playlistId , out var tracks ) ) {
            throw new InvalidOperationException( $"Playlist {playlistId} not found" );
        }

        offset ??= 0;
        limit ??= 50;

        var items = tracks.Items.ToList( );
        var paging = new PagingObject<Models.PlaylistItem> {
            Href = new Uri( $"{s_baseUri}/playlists/{playlistId}/tracks" ) ,
            Items = items.Skip( offset.Value ).Take( limit.Value ) ,
            Limit = limit.Value ,
            Next = offset.Value + limit.Value < items.Count ? new Uri( $"{s_baseUri}/playlists/{playlistId}/tracks?offset={offset.Value + limit.Value}&limit={limit.Value}" ) : null ,
            Offset = offset.Value ,
            Previous = offset.Value > 0 ? new Uri( $"{s_baseUri}/playlists/{playlistId}/tracks?offset={Math.Max( 0 , offset.Value - limit.Value )}&limit={limit.Value}" ) : null ,
            Total = items.Count
        };

        return Task.FromResult( paging );
    }

    public Task<PagingObject<Models.PlaylistItem>> GetPlaylistTracksAsync( string token , string url , CancellationToken ct ) {
        // Parse the URL to extract playlistId and parameters
        var uri = new Uri( url );
        var query = HttpUtility.ParseQueryString( uri.Query );
        var path = uri.PathAndQuery.Split( '/' );
        var playlistId = path[3]; // /playlists/{playlistId}/tracks

        var offset = int.TryParse( query["offset"] , out var o ) ? o : 0;
        var limit = int.TryParse( query["limit"] , out var l ) ? l : 100;

        return GetPlaylistTracksAsync( token , playlistId , offset , limit , ct );
    }

    public Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync( string token , string url , CancellationToken ct ) {
        // Parse the URL to extract parameters
        var uri = new Uri( url );
        var query = System.Web.HttpUtility.ParseQueryString( uri.Query );

        var offset = int.TryParse( query["offset"] , out var o ) ? o : 0;
        var limit = int.TryParse( query["limit"] , out var l ) ? l : 20;

        return GetCurrentUserPlaylistsAsync( token , offset , limit , ct );
    }

    private void InitializeTestData( ) {
        // Initialize artists
        InitializeArtists( );

        // Initialize albums
        InitializeAlbums( );

        // Initialize tracks
        InitializeTracks( );

        // Initialize playlists and their tracks
        InitializePlaylists( );
    }

    private void InitializeArtists( ) {
        var artists = new[]
        {
            new { Id = "artist001", Name = "The Beatles" } ,
            new { Id = "artist002", Name = "Pink Floyd" } ,
            new { Id = "artist003", Name = "Led Zeppelin" } ,
            new { Id = "artist004", Name = "Queen" } ,
            new { Id = "artist005", Name = "David Bowie" } ,
            new { Id = "artist006", Name = "Radiohead" } ,
            new { Id = "artist007", Name = "Nirvana" } ,
            new { Id = "artist008", Name = "Pearl Jam" }
        };

        foreach ( var artist in artists ) {
            _artists[artist.Id] = new Artist { Id = artist.Id , Name = artist.Name };
        }
    }

    private void InitializeAlbums( ) {
        var albums = new[]
        {
            new { Id = "album001", Name = "Abbey Road", ArtistId = "artist001", TotalTracks = 17, AlbumType = "album", ReleaseDate = "1969-09-26" } ,
            new { Id = "album002", Name = "The Dark Side of the Moon", ArtistId = "artist002", TotalTracks = 10, AlbumType = "album", ReleaseDate = "1973-03-01" } ,
            new { Id = "album003", Name = "Led Zeppelin IV", ArtistId = "artist003", TotalTracks = 8, AlbumType = "album", ReleaseDate = "1971-11-08" } ,
            new { Id = "album004", Name = "A Night at the Opera", ArtistId = "artist004", TotalTracks = 12, AlbumType = "album", ReleaseDate = "1975-11-21" } ,
            new { Id = "album005", Name = "The Rise and Fall of Ziggy Stardust", ArtistId = "artist005", TotalTracks = 11, AlbumType = "album", ReleaseDate = "1972-06-16" } ,
            new { Id = "album006", Name = "OK Computer", ArtistId = "artist006", TotalTracks = 12, AlbumType = "album", ReleaseDate = "1997-05-21" } ,
            new { Id = "album007", Name = "Nevermind", ArtistId = "artist007", TotalTracks = 12, AlbumType = "album", ReleaseDate = "1991-09-24" } ,
            new { Id = "album008", Name = "Ten", ArtistId = "artist008", TotalTracks = 11, AlbumType = "album", ReleaseDate = "1991-08-27" } ,
            new { Id = "album009", Name = "In Rainbows", ArtistId = "artist006", TotalTracks = 10, AlbumType = "album", ReleaseDate = "2007-10-10" } ,
            new { Id = "album010", Name = "MTV Unplugged in New York", ArtistId = "artist007", TotalTracks = 14, AlbumType = "album", ReleaseDate = "1994-11-01" }
        };

        foreach ( var album in albums ) {
            var artist = _artists[album.ArtistId];
            _albums[album.Id] = new Album {
                AlbumType = album.AlbumType ,
                Artists = new List<Artist> { artist } ,
                Id = album.Id ,
                Name = album.Name ,
                ReleaseDate = album.ReleaseDate ,
                ReleaseDatePrecision = "day" ,
                TotalTracks = album.TotalTracks
            };
        }
    }

    private void InitializeTracks( ) {
        var tracks = new[]
        {
            new { Id = "track001", Name = "Come Together", TrackNumber = 1, DiscNumber = 1, DurationMs = 259000, AlbumId = "album001" } ,
            new { Id = "track002", Name = "Something", TrackNumber = 2, DiscNumber = 1, DurationMs = 183000, AlbumId = "album001" } ,
            new { Id = "track003", Name = "Here Comes the Sun", TrackNumber = 7, DiscNumber = 1, DurationMs = 185000, AlbumId = "album001" } ,
            new { Id = "track004", Name = "Speak to Me", TrackNumber = 1, DiscNumber = 1, DurationMs = 68000, AlbumId = "album002" } ,
            new { Id = "track005", Name = "Time", TrackNumber = 4, DiscNumber = 1, DurationMs = 413000, AlbumId = "album002" } ,
            new { Id = "track006", Name = "Money", TrackNumber = 6, DiscNumber = 1, DurationMs = 382000, AlbumId = "album002" } ,
            new { Id = "track007", Name = "Black Dog", TrackNumber = 1, DiscNumber = 1, DurationMs = 296000, AlbumId = "album003" } ,
            new { Id = "track008", Name = "Stairway to Heaven", TrackNumber = 4, DiscNumber = 1, DurationMs = 482000, AlbumId = "album003" } ,
            new { Id = "track009", Name = "Rock and Roll", TrackNumber = 2, DiscNumber = 1, DurationMs = 220000, AlbumId = "album003" } ,
            new { Id = "track010", Name = "Bohemian Rhapsody", TrackNumber = 11, DiscNumber = 1, DurationMs = 355000, AlbumId = "album004" } ,
            new { Id = "track011", Name = "You're My Best Friend", TrackNumber = 4, DiscNumber = 1, DurationMs = 170000, AlbumId = "album004" } ,
            new { Id = "track012", Name = "Starman", TrackNumber = 4, DiscNumber = 1, DurationMs = 255000, AlbumId = "album005" } ,
            new { Id = "track013", Name = "Ziggy Stardust", TrackNumber = 9, DiscNumber = 1, DurationMs = 194000, AlbumId = "album005" } ,
            new { Id = "track014", Name = "Paranoid Android", TrackNumber = 2, DiscNumber = 1, DurationMs = 386000, AlbumId = "album006" } ,
            new { Id = "track015", Name = "Karma Police", TrackNumber = 6, DiscNumber = 1, DurationMs = 264000, AlbumId = "album006" } ,
            new { Id = "track016", Name = "No Surprises", TrackNumber = 10, DiscNumber = 1, DurationMs = 228000, AlbumId = "album006" } ,
            new { Id = "track017", Name = "Smells Like Teen Spirit", TrackNumber = 1, DiscNumber = 1, DurationMs = 301000, AlbumId = "album007" } ,
            new { Id = "track018", Name = "Come as You Are", TrackNumber = 3, DiscNumber = 1, DurationMs = 219000, AlbumId = "album007" } ,
            new { Id = "track019", Name = "Lithium", TrackNumber = 5, DiscNumber = 1, DurationMs = 257000, AlbumId = "album007" } ,
            new { Id = "track020", Name = "Even Flow", TrackNumber = 2, DiscNumber = 1, DurationMs = 293000, AlbumId = "album008" } ,
            new { Id = "track021", Name = "Alive", TrackNumber = 4, DiscNumber = 1, DurationMs = 341000, AlbumId = "album008" } ,
            new { Id = "track022", Name = "Jeremy", TrackNumber = 7, DiscNumber = 1, DurationMs = 318000, AlbumId = "album008" } ,
            new { Id = "track023", Name = "15 Step", TrackNumber = 1, DiscNumber = 1, DurationMs = 237000, AlbumId = "album009" } ,
            new { Id = "track024", Name = "Weird Fishes/Arpeggi", TrackNumber = 4, DiscNumber = 1, DurationMs = 318000, AlbumId = "album009" } ,
            new { Id = "track025", Name = "Reckoner", TrackNumber = 7, DiscNumber = 1, DurationMs = 290000, AlbumId = "album009" } ,
            new { Id = "track026", Name = "About a Girl", TrackNumber = 1, DiscNumber = 1, DurationMs = 217000, AlbumId = "album010" } ,
            new { Id = "track027", Name = "The Man Who Sold the World", TrackNumber = 6, DiscNumber = 1, DurationMs = 258000, AlbumId = "album010" }
        };

        foreach ( var track in tracks ) {
            var album = _albums[track.AlbumId];
            var artist = album.Artists.First( );
            _tracks[track.Id] = new Track {
                Album = album ,
                Artists = new List<Artist> { artist } ,
                DiscNumber = track.DiscNumber ,
                DurationMs = track.DurationMs ,
                Id = track.Id ,
                Name = track.Name ,
                TrackNumber = track.TrackNumber
            };
        }
    }

    private void InitializePlaylists( ) {
        var owner = new PublicUserObject {
            DisplayName = "Test User" ,
            ExternalUrls = new ExternalUrlObject { Spotify = new Uri( $"{s_spotifyUri}/user/testuser" ) } ,
            Followers = new FollowersObject { Total = 10 } ,
            Href = new Uri( $"{s_baseUri}/users/testuser" ) ,
            Id = "testuser" ,
            Type = "user" ,
            Uri = new Uri( "spotify:user:testuser" )
        };

        // Playlist 1: Classic Rock Essentials (15 tracks, 7 albums)
        var playlist1Id = "playlist001";
        var playlist1Tracks = new[] { "track001", "track002", "track003", "track004", "track005", "track006", "track007", "track008", "track010", "track012", "track013", "track020", "track021", "track017", "track018" };
        _playlists[playlist1Id] = CreatePlaylist( playlist1Id , "Classic Rock Essentials" , "The best classic rock tracks" , true , 15 , owner , "snap001" );
        _playlistTracks[playlist1Id] = CreatePlaylistTracksForPlaylist( playlist1Id , playlist1Tracks );

        // Playlist 2: 90s Alternative (9 tracks, 3 albums)
        var playlist2Id = "playlist002";
        var playlist2Tracks = new[] { "track014", "track015", "track016", "track017", "track018", "track019", "track020", "track021", "track022" };
        _playlists[playlist2Id] = CreatePlaylist( playlist2Id , "90s Alternative" , "Alternative rock from the 90s" , false , 9 , owner , "snap002" );
        _playlistTracks[playlist2Id] = CreatePlaylistTracksForPlaylist( playlist2Id , playlist2Tracks );

        // Playlist 3: British Invasion (8 tracks, 5 albums)
        var playlist3Id = "playlist003";
        var playlist3Tracks = new[] { "track001", "track002", "track003", "track004", "track005", "track010", "track012", "track014" };
        _playlists[playlist3Id] = CreatePlaylist( playlist3Id , "British Invasion" , "UK bands that changed music" , false , 8 , owner , "snap003" );
        _playlistTracks[playlist3Id] = CreatePlaylistTracksForPlaylist( playlist3Id , playlist3Tracks );

        // Playlist 4: Queue Test Playlist (3 tracks, 1 album - with _queue prefix)
        var playlist4Id = "playlist004";
        var playlist4Tracks = new[] { "track023", "track024", "track025" };
        _playlists[playlist4Id] = CreatePlaylist( playlist4Id , "_queue Test Playlist" , "Playlist with queue prefix for testing" , true , 3 , owner , "snap004" );
        _playlistTracks[playlist4Id] = CreatePlaylistTracksForPlaylist( playlist4Id , playlist4Tracks );

        // Playlist 5: Empty Playlist (0 tracks)
        var playlist5Id = "playlist005";
        _playlists[playlist5Id] = CreatePlaylist( playlist5Id , "Empty Playlist" , "A playlist with no tracks" , true , 0 , owner , "snap005" );
        _playlistTracks[playlist5Id] = new PagingObject<Models.PlaylistItem> {
            Href = new Uri( $"{s_baseUri}/playlists/{playlist5Id}/tracks" ) ,
            Items = new List<Models.PlaylistItem>( ) ,
            Limit = 100 ,
            Next = null ,
            Offset = 0 ,
            Previous = null ,
            Total = 0
        };
    }

    private SimplifiedPlaylistObject CreatePlaylist( string id , string name , string description , bool isPublic , int trackCount , PublicUserObject owner , string snapshotId ) {
        return new SimplifiedPlaylistObject {
            Collaborative = false ,
            Description = description ,
            ExternalUrls = new ExternalUrlObject { Spotify = new Uri( $"{s_spotifyUri}/playlist/{id}" ) } ,
            Images = new List<ImageObject> {
                new ImageObject { Height = 300 , Width = 300 , Url = new Uri( "https://example.com/playlist.jpg" ) }
            } ,
            Name = name ,
            Owner = owner ,
            Public = isPublic ,
            SnapshotId = snapshotId ,
            Tracks = new PlaylistTracksRefObject { Href = new Uri( $"{s_baseUri}/playlists/{id}/tracks" ) , Total = trackCount } ,
            Href = new Uri( $"{s_baseUri}/playlists/{id}" ) ,
            Id = id ,
            Type = ObjectType.Playlist ,
            Uri = new Uri( $"spotify:playlist:{id}" )
        };
    }

    private PagingObject<Models.PlaylistItem> CreatePlaylistTracksForPlaylist( string playlistId , string[] trackIds ) {
        var tracks = new List<Models.PlaylistItem>( );
        var baseDateTime = new DateTime( 2024 , 1 , 15 , 10 , 30 , 0 , DateTimeKind.Utc );

        for ( int i = 0; i < trackIds.Length; i++ ) {
            var trackId = trackIds[i];
            if ( _tracks.TryGetValue( trackId , out var track ) ) {
                tracks.Add( new Models.PlaylistItem {
                    AddedAt = baseDateTime.AddSeconds( i * 60 ) ,
                    Track = track
                } );
            }
        }

        return new PagingObject<Models.PlaylistItem> {
            Href = new Uri( $"{s_baseUri}/playlists/{playlistId}/tracks" ) ,
            Items = tracks ,
            Limit = 100 ,
            Next = null ,
            Offset = 0 ,
            Previous = null ,
            Total = tracks.Count
        };
    }
}
