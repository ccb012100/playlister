using System.Reflection;
using FluentMigrator.Runner;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Playlister.Configuration;
using Playlister.CQRS;
using Playlister.Middleware;
using Playlister.Models;
using Playlister.RefitClients;
using Playlister.Repositories;
using Playlister.Repositories.Implementations;
using Playlister.Utilities;
using Refit;

namespace Playlister.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration config,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.Configure<DebuggingOptions>(config.GetSection(DebuggingOptions.Debugging));
            }

            return services
                .Configure<SpotifyOptions>(config.GetSection(SpotifyOptions.Spotify))
                .Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.Database));
        }

        public static void ConfigureFluentMigrator(this IServiceCollection services)
        {
            string connectionString = services.BuildServiceProvider()
                .GetService<IOptions<DatabaseOptions>>()!.Value.ConnectionString;

            ServiceProvider serviceProvider = services
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddSQLite()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(Assembly.GetExecutingAssembly()).For.All())
                .AddLogging(c => c.AddFluentMigratorConsole())
                .BuildServiceProvider(false);

            using IServiceScope scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IServiceCollection AddRefitClients(this IServiceCollection services)
        {
            services.AddRefitClient<ISpotifyAccountsApi>(JsonUtility.SnakeCaseRefitSettings)
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.AccountsApiBaseAddress;
                })
                // .AddHttpMessageHandler<HttpLoggingMiddleware>()
                .AddHttpMessageHandler<HttpQueryStringConversionMiddleware>();

            services.AddRefitClient<ISpotifyApi>(JsonUtility.SnakeCaseRefitSettings)
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.ApiBaseAddress;
                })
                // .AddHttpMessageHandler<HttpLoggingMiddleware>()
                .AddHttpMessageHandler<SpotifyAuthHeaderMiddleware>()
                .AddHttpMessageHandler<HttpQueryStringConversionMiddleware>();

            return services;
        }

        public static void AddDebuggingOptions(this IServiceCollection services)
        {
            var debugOptions = services.BuildServiceProvider().GetService<IOptions<DebuggingOptions>>();

            if (debugOptions is not null && debugOptions.Value.UseLoggingBehavior)
            {
                services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            }
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services) =>
            services
                .AddScoped<IPlaylistReadRepository, PlaylistReadRepository>()
                .AddScoped<IPlaylistWriteRepository, PlaylistWriteRepository>()
                .AddScoped<IAccessTokenRepository, AccessTokenRepository>();

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IApplicationBuilder AddEndpoints(this IApplicationBuilder builder, IConfiguration config,
            IWebHostEnvironment env)
        {
            return builder.UseEndpoints(endpoints =>
            {
                if (env.IsDevelopment())
                    // view app settings at ~/debug
                    endpoints.MapGet("/debug", async context
                        => await context.Response.WriteAsync((config as IConfigurationRoot).GetDebugView()));

                endpoints.MapGet("/info", async context
                    => await context.Response.WriteAsJsonAsync(new AppInfo()));

                endpoints.MapControllers();
            });
        }
    }
}
