using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Playlister.Api.HttpClients;
using Refit;

namespace Playlister.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection
            AddConfigOptions(this IServiceCollection services, IConfiguration config) =>
            services.Configure<SpotifyOptions>(config.GetSection(SpotifyOptions.Spotify));

        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddRefitClient<ISpotifyApi>()
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.ApiBaseAddress;
                });

            services
                .AddRefitClient<ISpotifyAccountsApi>()
                .ConfigureHttpClient((svc, c) =>
                {
                    c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.AccountsApiBaseAddress;
                });

            return services;
        }
    }
}
