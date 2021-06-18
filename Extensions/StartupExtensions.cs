using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Playlister.HttpClients;
using Playlister.Middleware;
using Playlister.Models;
using Refit;

namespace Playlister.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration config) =>
            services
                .Configure<SpotifyOptions>(config.GetSection(SpotifyOptions.Spotify))
                .Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.Database));

        public static void ConfigureFluentMigrator(this IServiceCollection serviceCollection)
        {
            string dbName = serviceCollection.BuildServiceProvider()
                .GetService<IOptions<DatabaseOptions>>()!.Value.DbName;

            ServiceProvider? serviceProvider = serviceCollection
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddSQLite()
                    .WithGlobalConnectionString($"DataSource={dbName}")
                    .ScanIn(Assembly.GetExecutingAssembly()).For.All())
                .AddLogging(c => c.AddFluentMigratorConsole())
                .BuildServiceProvider(false);

            using IServiceScope? scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            /*
             * WARNING:
             * Refit settings aren't used on body params that are marked with
             * [Body(BodySerializationMethod.UrlEncoded)], so those models have to be set with [AliasAs] to get the
             * properties snake_cased
             */
            var snakeCaseSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(
                    new JsonSerializerSettings
                    {
                        ContractResolver =
                            new DefaultContractResolver
                            {
                                NamingStrategy = new SnakeCaseNamingStrategy()
                            }
                    })
            };

            services.AddRefitClient<ISpotifyAccountsApi>(snakeCaseSettings)
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.AccountsApiBaseAddress;
                })
                // .AddHttpMessageHandler<HttpLoggingMiddleware>()
                .AddHttpMessageHandler<HttpQueryStringConversionMiddleware>();

            services.AddRefitClient<ISpotifyApi>(snakeCaseSettings)
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.ApiBaseAddress;
                })
                // .AddHttpMessageHandler<HttpLoggingMiddleware>()
                .AddHttpMessageHandler<SpotifyAuthHeaderMiddleware>()
                .AddHttpMessageHandler<HttpQueryStringConversionMiddleware>();

            return services;
        }

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
