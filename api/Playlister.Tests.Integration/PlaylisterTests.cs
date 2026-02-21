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
