using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Playlister
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection
            AddConfigOptions(this IServiceCollection services, IConfiguration config) =>
            services.Configure<SpotifyOptions>(config.GetSection(SpotifyOptions.Spotify));
    }
}