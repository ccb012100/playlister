using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Playlister.Repositories;

namespace Playlister.Tests.Integration;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {
    private const string TestEnvironment = "Integration";
    private readonly string _sharedConnectionName = $"file:{Guid.NewGuid()}?mode=memory&cache=shared";
    private SqliteConnection? _sharedConnection;

    /// <summary>
    ///     Whether to seed the test database with sample data.
    /// </summary>
    public bool SeedDatabase { get; set; }

    protected override void ConfigureWebHost( IWebHostBuilder builder ) {
        // Set Environment
        Environment.SetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" , TestEnvironment );
        builder.UseEnvironment( TestEnvironment );

        // Apply config from appsettings.Integration.json
        builder.ConfigureAppConfiguration( ( _ , configBuilder ) => {
                configBuilder.AddJsonFile(
                    Path.Combine( Directory.GetCurrentDirectory( ) , $"appsettings.{TestEnvironment}.json" )
                );
            }
        );

        // Override the database connection string with an in-memory shared database
        IConfigurationRoot configuration = new ConfigurationBuilder( )
            .AddInMemoryCollection(
                new Dictionary<string , string?> {
                    { "Database:ConnectionString" , $"Data Source={_sharedConnectionName}" }
                }
            )
            .Build( );

        builder.ConfigureAppConfiguration( configBuilder => configBuilder.AddConfiguration( configuration ) )
            .ConfigureTestServices( services => {
                    // Keep a shared connection open to maintain the in-memory database
                    ServiceProvider sp = services.BuildServiceProvider( );
                    IConnectionFactory factory = sp.GetRequiredService<IConnectionFactory>( );
                    _sharedConnection = factory.Connection;
                    _sharedConnection.Open( );

                    // Initialize database schema and optionally seed data
                    if ( SeedDatabase ) {
                        TestDatabaseHelper.InitializeWithSeedData( _sharedConnection );
                    } else {
                        TestDatabaseHelper.InitializeSchema( _sharedConnection );
                    }
                }
            );
    }

    protected override void Dispose( bool disposing ) {
        if ( disposing ) {
            _sharedConnection?.Dispose( );
        }

        base.Dispose( disposing );
    }
}
