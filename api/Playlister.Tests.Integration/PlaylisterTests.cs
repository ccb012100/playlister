using Dapper;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Models;
using Playlister.Repositories;

namespace Playlister.Tests.Integration;

public class PlaylisterTests : IClassFixture<CustomWebApplicationFactory<Startup>> {
    private readonly HttpClient _client;
    private readonly SqliteConnection _db;
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly IPlaylistReadRepository _playlistReadRepository;

    public PlaylisterTests( CustomWebApplicationFactory<Startup> factory ) {
        factory.SeedDatabase = true;
        _factory = factory;

        _client = factory.CreateClient( new WebApplicationFactoryClientOptions { AllowAutoRedirect = false } );
        _db = factory.Services.GetService<IConnectionFactory>( )!.Connection;
        _playlistReadRepository = factory.Services.GetRequiredService<IPlaylistReadRepository>( );
    }

    [Fact]
    public async Task ConnectsToDatabase( ) {
        // arrange
        const string expectedTableName = "main";
        const string expectedTableFile = ""; // in-memory database will have empty string for `file` value

        const string connectionStringPattern
            = """Data Source=\"file:[-a-z0-9]+\?mode=memory&cache=shared\";Mode=ReadWriteCreate;Cache=Shared;Foreign Keys=True""";

        // act
        (string name , string file)[ ] results
            = ( await _db.QueryAsync<(string name , string file)>( "SELECT name, file from pragma_database_list" ) ).ToArray( );

        // assert
        results.Should( ).HaveCount( 1 );
        results.First( ).Should( ).Be( ( expectedTableName , expectedTableFile ) );
        _db.ConnectionString.Should( ).MatchRegex( connectionStringPattern );
    }

    [Fact]
    public async Task DatabaseIsSeededWithTestData( ) {
        // act
        int artistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Artist" );
        int albumCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Album" );
        int trackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Track" );
        int playlistCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM Playlist" );
        int playlistTrackCount = await _db.ExecuteScalarAsync<int>( "SELECT COUNT(*) FROM PlaylistTrack" );

        // assert - verify seed data counts match seed_db.sql
        artistCount.Should( ).Be( 8 , "seed data should have 8 artists" );
        albumCount.Should( ).Be( 10 , "seed data should have 10 albums" );
        trackCount.Should( ).Be( 27 , "seed data should have 27 tracks" );
        playlistCount.Should( ).Be( 5 , "seed data should have 5 playlists" );
        playlistTrackCount.Should( ).Be( 35 , "seed data should have 35 playlist track associations" );
    }

    [Fact]
    public async Task CanQueryPlaylistWithTracks( ) {
        // act
        IEnumerable<Playlist> playlists = ( await _playlistReadRepository.GetAllAsync( ) )
            .OrderByDescending( p => p.Count )
            .ToList( );

        // assert
        playlists.Should( ).HaveCount( 5 );
        playlists.First( ).Name.Should( ).Be( "Classic Rock Essentials" );
        playlists.First( ).Count.Should( ).Be( 15 );
        playlists.Last( ).Name.Should( ).Be( "Empty Playlist" );
        playlists.Last( ).Count.Should( ).Be( 0 );
    }
}
