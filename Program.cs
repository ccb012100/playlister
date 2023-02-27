using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Playlister.Utilities;

namespace Playlister
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        internal static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddFile(context.Configuration.GetSection("Logging"));
                })
                .UseKestrel(LogDevelopmentConfiguration)
                .UseStartup<Startup>();
        }

        private static void LogDevelopmentConfiguration(WebHostBuilderContext context, KestrelServerOptions options)
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                var debug = context.Configuration.GetChildren().First(c => c.Key == "Debugging");
                var printEnv = debug.GetChildren().First(x => x.Key == "PrintEnvironmentInfo");

                if (bool.Parse(printEnv.Value))
                {
                    ShowConfig(context.Configuration);
                }
            }
        }

        /// <summary>
        ///     Write App Configuration to Console
        /// </summary>
        /// <param name="configuration"></param>
        private static void ShowConfig(IConfiguration configuration)
        {
            var children = configuration.GetChildren().ToList();
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
