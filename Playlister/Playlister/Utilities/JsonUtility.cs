using System.Globalization;
using System.Text.Json;
using EFCore.NamingConventions.Internal;
using Refit;

namespace Playlister.Utilities
{
    public static class JsonUtility
    {
        private static JsonSerializerOptions SnakeCaseSerializerOptions =>
            new()
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            };

        public static RefitSettings SnakeCaseRefitSettings =>
            new()
            {
                ContentSerializer = new SystemTextJsonContentSerializer(SnakeCaseSerializerOptions)
            };

        /// <summary>
        /// Serialize object to pretty-printed JSON string.
        /// </summary>
        /// <param name="t">Object you want converted to JSON</param>
        /// <typeparam name="T">Type of <paramref name="t"/></typeparam>
        /// <returns>String representation of the object as pretty-printed JSON</returns>
        public static string PrettyPrint<T>(T t) =>
            JsonSerializer.Serialize(t, new JsonSerializerOptions { WriteIndented = true });

        /// <summary>
        /// Convert string to snake_case using EFCore's <see cref="SnakeCaseNameRewriter"/>
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>input string converted to snake_case</returns>
        public static string ConvertToSnakeCase(string s) =>
            new SnakeCaseNameRewriter(CultureInfo.InvariantCulture).RewriteName(s);
    }
}
