using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Playlister;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddFile(builder.Configuration.GetSection("Logging"));
        builder.WebHost.UseKestrel(LogDevelopmentConfiguration);

        Startup startup = new Startup(builder.Configuration, builder.Environment);

        startup.ConfigureServices(builder.Services);

        WebApplication app = builder.Build();

        startup.Configure(app, app.Lifetime);

        app.Run();
    }

    private static void LogDevelopmentConfiguration(
        WebHostBuilderContext context,
        KestrelServerOptions options
    )
    {
        if (!context.HostingEnvironment.IsDevelopment())
        {
            return;
        }

        IConfigurationSection debug = context.Configuration.GetChildren().First(c => c.Key == "Debugging");
        IConfigurationSection printEnv = debug.GetChildren().First(x => x.Key == "PrintEnvironmentInfo");

        if (bool.Parse(printEnv.Value!))
        {
            ShowConfig(context.Configuration);
        }
    }

    /// <summary>
    ///     Write App Configuration to Console
    /// </summary>
    /// <param name="configuration"></param>
    private static void ShowConfig(IConfiguration configuration)
    {
        List<IConfigurationSection> children = configuration.GetChildren().ToList();

        if (children.Count == 0)
        {
            return;
        }

        foreach (IConfigurationSection section in children)
        {
            ShowConfig(section);
            Console.WriteLine($"{section.Path} => {section.Value}");
        }
    }
}
