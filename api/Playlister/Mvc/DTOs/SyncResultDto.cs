namespace Playlister.DTOs;

public record SyncResultDto
{
    public required int Deleted { get; init; }
    public required int TotalSynced { get; init; }
    public required int Updated { get; init; }
    public required string Elapsed { get; init; }
}
