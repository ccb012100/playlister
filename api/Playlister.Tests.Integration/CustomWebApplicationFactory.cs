using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Playlister.Tests.Integration;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {
    protected override void ConfigureWebHost( IWebHostBuilder builder ) {
        Dictionary<string , string> configurationValues = new( ) {
            { "AccountsApiBaseAddress" , "https://localhost:5001/spotify-accounts" } ,
            { "ApiBaseAddress" , "https://localhost:5001/spotify/v1" } ,
            { "CallbackUrl" , "https://localhost:5001/login" } ,
            { "ClientId" , "TEST_CLIENT_ID_VALUE" } ,
            { "ClientSecret" , "TEST_CLIENT_SECRET_VALUE" } ,
            { "ConnectionString" , "Data Source=IntegrationTestDatabase;Mode=Memory;Cache=Shared" }
        };

        IConfigurationRoot configuration = new ConfigurationBuilder( )
            .AddInMemoryCollection( configurationValues! )
            .Build( );

        builder
            /*
             * This is applied _before_ Program.Main, which allows us to override `appsettings.json` before Services are configured.
             * This is necessary because FluentMigrator runs migrations on Startup, and we need to override the ConnectionString.
             */
            .UseConfiguration(
                configuration
            )
            .ConfigureTestServices(
                services => {
                    // register mocks here
                }
            );
    }
}
