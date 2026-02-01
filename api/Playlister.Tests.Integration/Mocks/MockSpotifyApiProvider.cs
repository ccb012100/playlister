using System.Collections.Specialized;
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

    private const int _limit = 50;

    private readonly Dictionary<string , SimplifiedPlaylistObject> _playlists;
    private readonly Dictionary<string , List<PlaylistItem>> _playlistTracks;

    public MockSpotifyApiProvider( ) {
        _playlists = [ ];
        _playlistTracks = [ ];

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
        if ( !_playlists.TryGetValue( playlistId , out SimplifiedPlaylistObject? playlist ) ) {
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
        limit ??= _limit;

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

    public Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync(
        string token ,
        string playlistId ,
        int? offset ,
        int? limit ,
        CancellationToken ct
    ) {
        if ( !_playlistTracks.TryGetValue( playlistId , out List<PlaylistItem>? allTracks ) ) {
            throw new InvalidOperationException( $"Playlist {playlistId} not found" );
        }

        offset ??= 0;
        limit ??= 50;

        var items = allTracks.ToList( );
        var paging = new PagingObject<PlaylistItem> {
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

    public Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync( string token , string url , CancellationToken ct ) {
        // Parse the URL to extract playlistId and parameters
        var uri = new Uri( url );
        NameValueCollection query = HttpUtility.ParseQueryString( uri.Query );
        string[ ] path = uri.PathAndQuery.Split( '/' );
        string playlistId = path[3]; // /playlists/{playlistId}/tracks

        int offset = int.TryParse( query["offset"] , out int o ) ? o : 0;
        int limit = int.TryParse( query["limit"] , out int l ) ? l : _limit;

        return GetPlaylistTracksAsync( token , playlistId , offset , limit , ct );
    }

    public Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync( string token , string url , CancellationToken ct ) {
        // Parse the URL to extract parameters
        var uri = new Uri( url );
        NameValueCollection query = System.Web.HttpUtility.ParseQueryString( uri.Query );

        int offset = int.TryParse( query["offset"] , out int o ) ? o : 0;
        int limit = int.TryParse( query["limit"] , out int l ) ? l : 50;

        return GetCurrentUserPlaylistsAsync( token , offset , limit , ct );
    }

    #region initialize with test data
    private void InitializeTestData( ) {
        InitializePlaylists( );
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

        // Build all artists inline
        var artists = new Dictionary<string , Artist> {
            { "artist001", new Artist { Id = "artist001", Name = "The Beatles" } } ,
            { "artist002", new Artist { Id = "artist002", Name = "Pink Floyd" } } ,
            { "artist003", new Artist { Id = "artist003", Name = "Led Zeppelin" } } ,
            { "artist004", new Artist { Id = "artist004", Name = "Queen" } } ,
            { "artist005", new Artist { Id = "artist005", Name = "David Bowie" } } ,
            { "artist006", new Artist { Id = "artist006", Name = "Radiohead" } } ,
            { "artist007", new Artist { Id = "artist007", Name = "Nirvana" } } ,
            { "artist008", new Artist { Id = "artist008", Name = "Pearl Jam" } }
        };

        // Build all albums inline
        var albums = new Dictionary<string , Album> {
            { "album001", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist001"] }, Id = "album001", Name = "Abbey Road", ReleaseDate = "1969-09-26", ReleaseDatePrecision = "day", TotalTracks = 17 } } ,
            { "album002", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist002"] }, Id = "album002", Name = "The Dark Side of the Moon", ReleaseDate = "1973-03-01", ReleaseDatePrecision = "day", TotalTracks = 10 } } ,
            { "album003", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist003"] }, Id = "album003", Name = "Led Zeppelin IV", ReleaseDate = "1971-11-08", ReleaseDatePrecision = "day", TotalTracks = 8 } } ,
            { "album004", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist004"] }, Id = "album004", Name = "A Night at the Opera", ReleaseDate = "1975-11-21", ReleaseDatePrecision = "day", TotalTracks = 12 } } ,
            { "album005", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist005"] }, Id = "album005", Name = "The Rise and Fall of Ziggy Stardust", ReleaseDate = "1972-06-16", ReleaseDatePrecision = "day", TotalTracks = 11 } } ,
            { "album006", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist006"] }, Id = "album006", Name = "OK Computer", ReleaseDate = "1997-05-21", ReleaseDatePrecision = "day", TotalTracks = 12 } } ,
            { "album007", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist007"] }, Id = "album007", Name = "Nevermind", ReleaseDate = "1991-09-24", ReleaseDatePrecision = "day", TotalTracks = 12 } } ,
            { "album008", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist008"] }, Id = "album008", Name = "Ten", ReleaseDate = "1991-08-27", ReleaseDatePrecision = "day", TotalTracks = 11 } } ,
            { "album009", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist006"] }, Id = "album009", Name = "In Rainbows", ReleaseDate = "2007-10-10", ReleaseDatePrecision = "day", TotalTracks = 10 } } ,
            { "album010", new Album { AlbumType = "album", Artists = new List<Artist> { artists["artist007"] }, Id = "album010", Name = "MTV Unplugged in New York", ReleaseDate = "1994-11-01", ReleaseDatePrecision = "day", TotalTracks = 14 } }
        };

        // Build all tracks inline
        var tracks = new Dictionary<string , Track> {
            { "track001", new Track { Album = albums["album001"], Artists = new List<Artist> { artists["artist001"] }, DiscNumber = 1, DurationMs = 259000, Id = "track001", Name = "Come Together", TrackNumber = 1 } } ,
            { "track002", new Track { Album = albums["album001"], Artists = new List<Artist> { artists["artist001"] }, DiscNumber = 1, DurationMs = 183000, Id = "track002", Name = "Something", TrackNumber = 2 } } ,
            { "track003", new Track { Album = albums["album001"], Artists = new List<Artist> { artists["artist001"] }, DiscNumber = 1, DurationMs = 185000, Id = "track003", Name = "Here Comes the Sun", TrackNumber = 7 } } ,
            { "track004", new Track { Album = albums["album002"], Artists = new List<Artist> { artists["artist002"] }, DiscNumber = 1, DurationMs = 68000, Id = "track004", Name = "Speak to Me", TrackNumber = 1 } } ,
            { "track005", new Track { Album = albums["album002"], Artists = new List<Artist> { artists["artist002"] }, DiscNumber = 1, DurationMs = 413000, Id = "track005", Name = "Time", TrackNumber = 4 } } ,
            { "track006", new Track { Album = albums["album002"], Artists = new List<Artist> { artists["artist002"] }, DiscNumber = 1, DurationMs = 382000, Id = "track006", Name = "Money", TrackNumber = 6 } } ,
            { "track007", new Track { Album = albums["album003"], Artists = new List<Artist> { artists["artist003"] }, DiscNumber = 1, DurationMs = 296000, Id = "track007", Name = "Black Dog", TrackNumber = 1 } } ,
            { "track008", new Track { Album = albums["album003"], Artists = new List<Artist> { artists["artist003"] }, DiscNumber = 1, DurationMs = 482000, Id = "track008", Name = "Stairway to Heaven", TrackNumber = 4 } } ,
            { "track009", new Track { Album = albums["album003"], Artists = new List<Artist> { artists["artist003"] }, DiscNumber = 1, DurationMs = 220000, Id = "track009", Name = "Rock and Roll", TrackNumber = 2 } } ,
            { "track010", new Track { Album = albums["album004"], Artists = new List<Artist> { artists["artist004"] }, DiscNumber = 1, DurationMs = 355000, Id = "track010", Name = "Bohemian Rhapsody", TrackNumber = 11 } } ,
            { "track011", new Track { Album = albums["album004"], Artists = new List<Artist> { artists["artist004"] }, DiscNumber = 1, DurationMs = 170000, Id = "track011", Name = "You're My Best Friend", TrackNumber = 4 } } ,
            { "track012", new Track { Album = albums["album005"], Artists = new List<Artist> { artists["artist005"] }, DiscNumber = 1, DurationMs = 255000, Id = "track012", Name = "Starman", TrackNumber = 4 } } ,
            { "track013", new Track { Album = albums["album005"], Artists = new List<Artist> { artists["artist005"] }, DiscNumber = 1, DurationMs = 194000, Id = "track013", Name = "Ziggy Stardust", TrackNumber = 9 } } ,
            { "track014", new Track { Album = albums["album006"], Artists = new List<Artist> { artists["artist006"] }, DiscNumber = 1, DurationMs = 386000, Id = "track014", Name = "Paranoid Android", TrackNumber = 2 } } ,
            { "track015", new Track { Album = albums["album006"], Artists = new List<Artist> { artists["artist006"] }, DiscNumber = 1, DurationMs = 264000, Id = "track015", Name = "Karma Police", TrackNumber = 6 } } ,
            { "track016", new Track { Album = albums["album006"], Artists = new List<Artist> { artists["artist006"] }, DiscNumber = 1, DurationMs = 228000, Id = "track016", Name = "No Surprises", TrackNumber = 10 } } ,
            { "track017", new Track { Album = albums["album007"], Artists = new List<Artist> { artists["artist007"] }, DiscNumber = 1, DurationMs = 301000, Id = "track017", Name = "Smells Like Teen Spirit", TrackNumber = 1 } } ,
            { "track018", new Track { Album = albums["album007"], Artists = new List<Artist> { artists["artist007"] }, DiscNumber = 1, DurationMs = 219000, Id = "track018", Name = "Come as You Are", TrackNumber = 3 } } ,
            { "track019", new Track { Album = albums["album007"], Artists = new List<Artist> { artists["artist007"] }, DiscNumber = 1, DurationMs = 257000, Id = "track019", Name = "Lithium", TrackNumber = 5 } } ,
            { "track020", new Track { Album = albums["album008"], Artists = new List<Artist> { artists["artist008"] }, DiscNumber = 1, DurationMs = 293000, Id = "track020", Name = "Even Flow", TrackNumber = 2 } } ,
            { "track021", new Track { Album = albums["album008"], Artists = new List<Artist> { artists["artist008"] }, DiscNumber = 1, DurationMs = 341000, Id = "track021", Name = "Alive", TrackNumber = 4 } } ,
            { "track022", new Track { Album = albums["album008"], Artists = new List<Artist> { artists["artist008"] }, DiscNumber = 1, DurationMs = 318000, Id = "track022", Name = "Jeremy", TrackNumber = 7 } } ,
            { "track023", new Track { Album = albums["album009"], Artists = new List<Artist> { artists["artist006"] }, DiscNumber = 1, DurationMs = 237000, Id = "track023", Name = "15 Step", TrackNumber = 1 } } ,
            { "track024", new Track { Album = albums["album009"], Artists = new List<Artist> { artists["artist006"] }, DiscNumber = 1, DurationMs = 318000, Id = "track024", Name = "Weird Fishes/Arpeggi", TrackNumber = 4 } } ,
            { "track025", new Track { Album = albums["album009"], Artists = new List<Artist> { artists["artist006"] }, DiscNumber = 1, DurationMs = 290000, Id = "track025", Name = "Reckoner", TrackNumber = 7 } } ,
            { "track026", new Track { Album = albums["album010"], Artists = new List<Artist> { artists["artist007"] }, DiscNumber = 1, DurationMs = 217000, Id = "track026", Name = "About a Girl", TrackNumber = 1 } } ,
            { "track027", new Track { Album = albums["album010"], Artists = new List<Artist> { artists["artist007"] }, DiscNumber = 1, DurationMs = 258000, Id = "track027", Name = "The Man Who Sold the World", TrackNumber = 6 } }
        };

        // Playlist 1: Classic Rock Essentials (15 tracks, 7 albums)
        string playlist1Id = "playlist001";
        string[ ] playlist1Tracks = new[ ] { "track001" , "track002" , "track003" , "track004" , "track005" , "track006" , "track007" , "track008" , "track010" , "track012" , "track013" , "track020" , "track021" , "track017" , "track018" };
        _playlists[playlist1Id] = CreatePlaylist( playlist1Id , "Classic Rock Essentials" , "The best classic rock tracks" , true , 15 , owner , "snap001" );
        _playlistTracks[playlist1Id] = CreatePlaylistTracksForPlaylist( playlist1Id , playlist1Tracks , tracks );

        // Playlist 2: 90s Alternative (9 tracks, 3 albums)
        string playlist2Id = "playlist002";
        string[ ] playlist2Tracks = new[ ] { "track014" , "track015" , "track016" , "track017" , "track018" , "track019" , "track020" , "track021" , "track022" };
        _playlists[playlist2Id] = CreatePlaylist( playlist2Id , "90s Alternative" , "Alternative rock from the 90s" , false , 9 , owner , "snap002" );
        _playlistTracks[playlist2Id] = CreatePlaylistTracksForPlaylist( playlist2Id , playlist2Tracks , tracks );

        // Playlist 3: British Invasion (8 tracks, 5 albums)
        string playlist3Id = "playlist003";
        string[ ] playlist3Tracks = new[ ] { "track001" , "track002" , "track003" , "track004" , "track005" , "track010" , "track012" , "track014" };
        _playlists[playlist3Id] = CreatePlaylist( playlist3Id , "British Invasion" , "UK bands that changed music" , false , 8 , owner , "snap003" );
        _playlistTracks[playlist3Id] = CreatePlaylistTracksForPlaylist( playlist3Id , playlist3Tracks , tracks );

        // Playlist 4: Queue Test Playlist (3 tracks, 1 album - with _queue prefix)
        string playlist4Id = "playlist004";
        string[ ] playlist4Tracks = new[ ] { "track023" , "track024" , "track025" };
        _playlists[playlist4Id] = CreatePlaylist( playlist4Id , "_queue Test Playlist" , "Playlist with queue prefix for testing" , true , 3 , owner , "snap004" );
        _playlistTracks[playlist4Id] = CreatePlaylistTracksForPlaylist( playlist4Id , playlist4Tracks , tracks );

        // Playlist 5: Empty Playlist (0 tracks)
        string playlist5Id = "playlist005";
        _playlists[playlist5Id] = CreatePlaylist( playlist5Id , "Empty Playlist" , "A playlist with no tracks" , true , 0 , owner , "snap005" );
        _playlistTracks[playlist5Id] = new List<PlaylistItem>( );
    }

    private SimplifiedPlaylistObject CreatePlaylist( string id , string name , string description , bool isPublic , int trackCount , PublicUserObject owner , string snapshotId ) {
        return new SimplifiedPlaylistObject {
            Collaborative = false ,
            Description = description ,
            ExternalUrls = new ExternalUrlObject { Spotify = new Uri( $"{s_spotifyUri}/playlist/{id}" ) } ,
            Images = [
                    new ImageObject { Height = 300 , Width = 300 , Url = new Uri( "https://example.com/playlist.jpg" ) }
                ] ,
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

    private List<PlaylistItem> CreatePlaylistTracksForPlaylist( string playlistId , string[ ] trackIds , Dictionary<string , Track> tracks ) {
        var playlistItems = new List<PlaylistItem>( );
        var baseDateTime = new DateTime( 2024 , 1 , 15 , 10 , 30 , 0 , DateTimeKind.Utc );

        for ( int i = 0 ; i < trackIds.Length ; i++ ) {
            string trackId = trackIds[i];
            if ( tracks.TryGetValue( trackId , out Track? track ) ) {
                playlistItems.Add( new PlaylistItem {
                    AddedAt = baseDateTime.AddSeconds( i * 60 ) ,
                    Track = track
                } );
            }
        }

        return playlistItems;
    }
    #endregion
}
