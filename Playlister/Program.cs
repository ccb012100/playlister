using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Playlister
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        internal static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureLogging(
                    (context, builder) =>
                    { builder.AddFile(context.Configuration.GetSection("Logging")); }
                )
                .UseKestrel(LogDevelopmentConfiguration)
                .UseStartup<Startup>();
        }

        private static void LogDevelopmentConfiguration(
            WebHostBuilderContext context,
            KestrelServerOptions options
        )
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                IConfigurationSection debug = context.Configuration.GetChildren().First(c => c.Key == "Debugging");
                IConfigurationSection printEnv = debug.GetChildren().First(x => x.Key == "PrintEnvironmentInfo");

                if (bool.Parse(printEnv.Value!))
                { ShowConfig(context.Configuration); }
            }
        }

        /// <summary>
        ///     Write App Configuration to Console
        /// </summary>
        /// <param name="configuration"></param>
        private static void ShowConfig(IConfiguration configuration)
        {
            List<IConfigurationSection> children = configuration.GetChildren().ToList();

            if (children.Any())
            {
                foreach (IConfigurationSection section in children)
                {
                    ShowConfig(section);
                    Console.WriteLine($"{section.Path} => {section.Value}");
                }
            }
        }
    }
}
