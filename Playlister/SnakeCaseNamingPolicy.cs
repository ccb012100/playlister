using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace Playlister
{
    /// <summary>
    /// Uses Json.NET's snake_case converter.
    /// Code from https://stackoverflow.com/a/58575386
    /// </summary>
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        private readonly SnakeCaseNamingStrategy _newtonsoftSnakeCaseNamingStrategy = new();

        public static SnakeCaseNamingPolicy Instance { get; } = new();

        public override string ConvertName(string name) =>
            _newtonsoftSnakeCaseNamingStrategy.GetPropertyName(name, false);
    }
}
