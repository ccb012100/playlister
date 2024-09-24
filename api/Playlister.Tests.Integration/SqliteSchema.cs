using System.Text.Json.Serialization;

namespace Playlister.Tests.Integration;

internal record SqliteSchema {
    public required string Type { get; init; }
    public required string Name { get; init; }
    [JsonPropertyName( "tbl_name" )] public required string TblName { get; init; }
    public required int RootPage { get; init; }
    public required string Sql { get; init; }
}
