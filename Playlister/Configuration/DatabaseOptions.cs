#pragma warning disable 8618
namespace Playlister.Configuration
{
    public record DatabaseOptions
    {
        public const string Database = "Database";

        public string ConnectionString { get; init; }
    }
}
