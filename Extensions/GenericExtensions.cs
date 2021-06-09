using Newtonsoft.Json;

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
        public static string ToPrettyPrintJson<T>(this T t) => JsonConvert.SerializeObject(t, Formatting.Indented);
    }
}
