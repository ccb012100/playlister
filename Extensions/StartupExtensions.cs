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
using Refit;

namespace Playlister.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection
            AddConfigOptions(this IServiceCollection services, IConfiguration config)
        {
            return services.Configure<SpotifyOptions>(config.GetSection(SpotifyOptions.Spotify));
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddRefitClient<ISpotifyApi>()
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.ApiBaseAddress;
                });

            services.AddRefitClient<ISpotifyAccountsApi>(new RefitSettings
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
                })
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.AccountsApiBaseAddress;
                });

            return services;
        }

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