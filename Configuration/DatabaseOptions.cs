#pragma warning disable 8618
namespace Playlister.Configuration
{
    public record DatabaseOptions
    {
        public const string Database = "Database";

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string DbName { get; init; }
    }
}
