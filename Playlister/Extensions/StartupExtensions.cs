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
using Playlister.Configuration;
using Playlister.CQRS;
using Playlister.Middleware;
using Playlister.Models;
using Playlister.RefitClients;
using Playlister.Repositories;
using Playlister.Repositories.Implementations;
using Playlister.Services;
using Playlister.Utilities;
using Refit;

namespace Playlister.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddConfigOptions(
            this IServiceCollection services,
            IConfiguration config,
            IWebHostEnvironment env
        )
        {
            if (env.IsDevelopment())
            {
                services.Configure<DebuggingOptions>(config.GetSection(DebuggingOptions.Debugging));
            }

            return services
                .Configure<SpotifyOptions>(config.GetSection(SpotifyOptions.Spotify))
                .Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.Database));
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConnectionFactory, ConnectionFactory>()
                .AddScoped<IPlaylistService, PlaylistService>()
                .AddTransient<IAccessTokenUtility, AccessTokenUtility>();
        }

        public static IServiceCollection AddMiddleware(this IServiceCollection services)
        {
            return services
                .AddTransient<HttpLoggingMiddleware>()
                .AddTransient<SpotifyAuthHeaderMiddleware>();
        }

        public static void ConfigureFluentMigrator(this IServiceCollection services)
        {
            string connectionString = services
                .BuildServiceProvider()
                .GetService<IOptions<DatabaseOptions>>()!
                .Value.ConnectionString;

            ServiceProvider serviceProvider = services
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    c =>
                        c.AddSQLite()
                            .WithGlobalConnectionString(connectionString)
                            .ScanIn(Assembly.GetExecutingAssembly())
                            .For.All()
                )
                .AddLogging(c => c.AddFluentMigratorConsole())
                .BuildServiceProvider(false);

            using IServiceScope scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
        }

        public static IServiceCollection AddRefitClients(this IServiceCollection services)
        {
            IOptions<DebuggingOptions>? debugOptions = services.BuildServiceProvider().GetService<IOptions<DebuggingOptions>>();

            services
                .AddRefitClient<ISpotifyAccountsApi>(JsonUtility.SnakeCaseRefitSettings)
                .ConfigureHttpClient(
                    (svc, c) =>
                    {
                        c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.AccountsApiBaseAddress;
                    }
                )
                .AddHttpLoggingMiddleware(debugOptions)
                .AddPolicyHandler(PollyUtility.RetryAfterPolicy);

            services
                .AddRefitClient<ISpotifyApi>(JsonUtility.SnakeCaseRefitSettings)
                .ConfigureHttpClient(
                    (svc, c) =>
                    {
                        c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.ApiBaseAddress;
                    }
                )
                .AddHttpLoggingMiddleware(debugOptions)
                .AddPolicyHandler(PollyUtility.RetryAfterPolicy);

            return services;
        }

        private static IHttpClientBuilder AddHttpLoggingMiddleware(
            this IHttpClientBuilder httpClientBuilder,
            IOptions<DebuggingOptions>? debugOptions
        )
        {
            if (debugOptions is { } && debugOptions.Value.UseHttpLoggingMiddleware)
            {
                httpClientBuilder.AddHttpMessageHandler<HttpLoggingMiddleware>();
            }

            return httpClientBuilder;
        }

        public static void AddDebuggingOptions(this IServiceCollection services)
        {
            IOptions<DebuggingOptions>? debugOptions = services.BuildServiceProvider().GetService<IOptions<DebuggingOptions>>();

            if (debugOptions is not null && debugOptions.Value.UseLoggingBehavior)
            {
                services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            }
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services) =>
            // these need to be added as Transient to prevent DI exceptions in Mediatr
            services
                .AddTransient<IPlaylistReadRepository, PlaylistReadRepository>()
                .AddTransient<IPlaylistWriteRepository, PlaylistWriteRepository>();

        public static IApplicationBuilder AddEndpoints(
            this IApplicationBuilder builder,
            IConfiguration config,
            IWebHostEnvironment env
        )
        {
            return builder.UseEndpoints(endpoints =>
            {
                if (env.IsDevelopment())
                    // view app settings at ~/debug
                    endpoints.MapGet(
                        "/debug",
                        async context =>
                            await context.Response.WriteAsync(
                                (config as IConfigurationRoot)!.GetDebugView()
                            )
                    );

                endpoints.MapGet(
                    "/info",
                    async context => await context.Response.WriteAsJsonAsync(new AppInfo())
                );

                endpoints.MapControllers();
            });
        }

        public static IServiceCollection AddHttpClientWithPollyPolicy(
            this IServiceCollection services
        )
        {
            services
                .AddHttpClient<ISpotifyApiService, SpotifyApiService>()
                .AddPolicyHandler(PollyUtility.RetryAfterPolicy);

            return services;
        }
    }
}
