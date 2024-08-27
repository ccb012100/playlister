using System.ComponentModel.DataAnnotations;

namespace Playlister.Configuration
{
    public record DatabaseOptions
    {
        public const string Database = "Database";

        [Required] public required string ConnectionString { get; init; }
    }
}
