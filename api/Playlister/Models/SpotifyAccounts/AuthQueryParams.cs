namespace Playlister.Models.SpotifyAccounts;

public record AuthQueryParams
{
    public required string ClientId { get; init; }
    public required Uri RedirectUri { get; init; }
    public required string State { get; init; }
    public string? Scope { get; init; }
    public bool ShowDialog { get; init; }

    public string ResponseType { get; init; } = "code";
}
