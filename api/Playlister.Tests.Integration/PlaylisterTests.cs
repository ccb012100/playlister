using System.Text.RegularExpressions;

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
    public async Task ConnectsToDatabase( ) {
        // arrange
        const string expectedTableName = "main";
        const string expectedTableFile = ""; // in-memory database will have empty string for `file` value

        const string connectionStringPattern
            = """Data Source=\"file:[-a-z0-9]+\?mode=memory&cache=shared\";Mode=ReadWriteCreate;Cache=Shared;Foreign Keys=True""";

        // act
        (string name, string file)[ ] results
            = ( await _db.QueryAsync<(string name, string file)>( "SELECT name, file from pragma_database_list" ) ).ToArray( );

        // assert
        results.Should( ).HaveCount( 1 );
        results.First( ).Should( ).Be( (expectedTableName, expectedTableFile) );
        _db.ConnectionString.Should( ).MatchRegex( connectionStringPattern );
    }
}
