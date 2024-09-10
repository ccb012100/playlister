using Microsoft.AspNetCore.Server.Kestrel.Core;
using Playlister.Configuration;

namespace Playlister;

public static class Program
{
    public static void Main( string[] args )
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

        builder.Logging.AddFile( builder.Configuration.GetSection( "Logging" ) );
        builder.WebHost.UseKestrel( PrintDevelopmentConfiguration );

        Startup.ConfigureWebApplication( Startup.ConfigureServices( builder.AddAndValidateConfiguration() ).Build() ).Run();
    }

    /// <summary>
    ///     If enabled, print the Application's Configuration to the Console
    /// </summary>
    /// <param name="context"></param>
    /// <param name="options"></param>
    private static void PrintDevelopmentConfiguration(
        WebHostBuilderContext context,
        KestrelServerOptions options
    )
    {
        if (context.HostingEnvironment.IsDevelopment() && context.Configuration.Get<DebuggingOptions>() is { PrintEnvironmentInfo: true })
        {
            WriteToConsole( context.Configuration );
        }

        return;

        void WriteToConsole( IConfiguration configuration )
        {
            List<IConfigurationSection> children = configuration.GetChildren().ToList();

            if (children.Count == 0)
            {
                return;
            }

            foreach (IConfigurationSection section in children)
            {
                WriteToConsole( section );
                Console.WriteLine( $"{section.Path} => {section.Value}" );
            }
        }
    }
}
