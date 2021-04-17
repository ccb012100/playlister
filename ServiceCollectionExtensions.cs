using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Playlister.HttpClients;
using Refit;

namespace Playlister
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
                .ConfigureHttpClient(c => c.BaseAddress = config.Get<SpotifyOptions>().ApiBaseUrl);

            services
                .AddRefitClient<ISpotifyAuthorizationApi>()
                .ConfigureHttpClient(c => c.BaseAddress = config.Get<SpotifyOptions>().AuthorizationUrl);

            return services;
        }
    }
}
