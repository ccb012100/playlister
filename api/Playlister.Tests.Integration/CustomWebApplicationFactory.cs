using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Playlister.RefitClients;
using Playlister.Repositories;
using Playlister.Tests.Integration.Mocks;
using Playlister.Tests.Integration.Utilities;

namespace Playlister.Tests.Integration;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {
    private const string TestEnvironment = "Integration";
    private SqliteConnection? _sharedConnection;

    /// <summary>
    ///     Whether to seed the test database with sample data.
    /// </summary>
    public bool SeedDatabase { get; set; }

    /// <summary>
    ///     Whether to use mock Spotify API. Default is true.
    /// </summary>
    public bool UseMockSpotifyApi { get; set; } = true;

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
        // We use a shared in-memory database per factory instance, so we use NewGuid() to allow tests in parallel
        IConfigurationRoot configuration = new ConfigurationBuilder( )
            .AddInMemoryCollection(
                new Dictionary<string , string?> {
                    { "Database:ConnectionString" , $"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared" }
                }
            )
            .Build( );

        builder.ConfigureAppConfiguration( configBuilder => configBuilder.AddConfiguration( configuration ) )
            .ConfigureTestServices( services => {
                // Replace the Spotify API clients with mocks if enabled
                if ( UseMockSpotifyApi ) {
                    // Remove existing Refit client registrations
                    var descriptorsToRemove = services
                        .Where( d => d.ServiceType == typeof( ISpotifyApi ) || d.ServiceType == typeof( ISpotifyAccountsApi ) )
                        .ToList( );

                    foreach ( var descriptor in descriptorsToRemove ) {
                        services.Remove( descriptor );
                    }

                    // Add mock implementations
                    services.AddSingleton<ISpotifyApi>( new MockSpotifyApiProvider( ) );
                    services.AddSingleton<ISpotifyAccountsApi>( new MockSpotifyAccountsApi( ) );
                }

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
