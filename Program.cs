using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

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
                    // Remove Console logger
                    foreach (var sd in builder.Services.Where(s => s.ImplementationType == typeof(ConsoleLoggerProvider)))
                    {
                        builder.Services.Remove(sd);
                        break;
                    }
                    builder.AddFile(context.Configuration.GetSection("Logging"));
                })
                .UseKestrel(LogDevelopmentConfiguration)
                .UseStartup<Startup>();
        }

        private static void LogDevelopmentConfiguration(WebHostBuilderContext context, KestrelServerOptions options)
        {
            if (context.HostingEnvironment.IsDevelopment()) ShowConfig(context.Configuration);
        }

        /// <summary>
        ///     Write App Configuration to Console
        /// </summary>
        /// <param name="configuration"></param>
        private static void ShowConfig(IConfiguration configuration)
        {
            foreach (IConfigurationSection pair in configuration.GetChildren())
            {
                Console.WriteLine($"{pair.Path} - {pair.Value}");
                ShowConfig(pair);
            }
        }
    }
}
