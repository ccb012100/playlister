using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Playlister.Tests.Integration;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {
    private const string TestEnvironment = "Integration";

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
                services => { }
            );
    }
}
