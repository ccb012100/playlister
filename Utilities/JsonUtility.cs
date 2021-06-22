using System.Text.Json;
using Refit;

namespace Playlister.Utilities
{
    public static class JsonUtility
    {
        public static JsonSerializerOptions SnakeCaseSerializerOptions =>
            new()
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            };

        public static RefitSettings SnakeCaseRefitSettings =>
            new()
            {
                ContentSerializer = new SystemTextJsonContentSerializer(SnakeCaseSerializerOptions)
            };
    }
}
