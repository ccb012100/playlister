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

    /// <summary>
    ///     Whether to seed the test database with sample data. Default is true.
    /// </summary>
    public bool SeedDatabase { get; set; } = true;

    protected override void ConfigureWebHost( IWebHostBuilder builder ) {
        // Set Environment
        // Set for `Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")`
        Environment.SetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" , TestEnvironment );
        // Set for `WebApplicationBuilder.Environment.EnvironmentName`
        builder.UseEnvironment( TestEnvironment );

        // Apply config from appsettings.Integration.json
        builder.ConfigureAppConfiguration(
            ( _ , configBuilder ) => {
                configBuilder.AddJsonFile(
                    Path.Combine( Directory.GetCurrentDirectory( ) , $"appsettings.{TestEnvironment}.json" )
                );
            }
        );

        IConfigurationRoot configuration = new ConfigurationBuilder( )
            .AddInMemoryCollection(
                new Dictionary<string , string?> { { "Database:ConnectionString" , $"Data Source=file:{Guid.NewGuid( )}?mode=memory&cache=shared" } }
            )
            .Build( );

        /*
         * UseConfiguration is applied _before_ Program.Main, which allows us to override `appsettings.json` before Services are configured.
         * This allows us to use a unique in-memory database for every test run, which is necessary to run tests in parallel without using the same connection.
         */
        builder.UseConfiguration( configuration )
            .ConfigureTestServices( // register mocks here
                services => {
                    // Initialize database after services are configured
                    ServiceProvider sp = services.BuildServiceProvider( );
                    SqliteConnection connection = sp.GetRequiredService<IConnectionFactory>( ).Connection;

                    if ( SeedDatabase ) {
                        TestDatabaseHelper.InitializeWithSeedData( connection );
                    } else {
                        TestDatabaseHelper.InitializeSchema( connection );
                    }
                }
            );
    }
}
