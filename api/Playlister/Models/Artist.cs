namespace Playlister.Models;

public record Artist {
    public required string Id { get; init; }

    public required string Name { get; init; }
}
