using System.Globalization;
using System.Text.Json;
using EFCore.NamingConventions.Internal;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Playlister.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Serialize object as pretty-printed JSON string.
        /// </summary>
        /// <param name="t">Object you want converted to JSON</param>
        /// <typeparam name="T">Type of <paramref name="t"/></typeparam>
        /// <returns>String representation of the object as pretty-printed JSON</returns>
        public static string ToPrettyPrintJson<T>(this T t) =>
            JsonSerializer.Serialize(t, new JsonSerializerOptions {WriteIndented = true});

        /// <summary>
        /// Convert string to snake_case using EFCore's <see cref="SnakeCaseNameRewriter"/>
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>input string converted to snake_case</returns>
        public static string ToSnakeCase(this string s) =>
            new SnakeCaseNameRewriter(CultureInfo.InvariantCulture).RewriteName(s);
    }
}
