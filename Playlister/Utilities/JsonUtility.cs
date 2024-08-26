using System.Text.Json;
using Refit;

namespace Playlister.Utilities
{
    public static class JsonUtility
    {
        private static readonly JsonSerializerOptions s_prettyPrintOptions = new() { WriteIndented = true };

        private static JsonSerializerOptions SnakeCaseSerializerOptions => new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        public static RefitSettings SnakeCaseRefitSettings =>
            new() { ContentSerializer = new SystemTextJsonContentSerializer(SnakeCaseSerializerOptions) };

        /// <summary>
        ///     Serialize object to pretty-printed JSON string.
        /// </summary>
        /// <param name="t">Object you want converted to JSON</param>
        /// <typeparam name="T">Type of <paramref name="t" /></typeparam>
        /// <returns>String representation of the object as pretty-printed JSON</returns>
        public static string PrettyPrint<T>(T t) => JsonSerializer.Serialize(t, s_prettyPrintOptions);
    }
}
