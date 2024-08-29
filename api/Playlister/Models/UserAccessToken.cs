using Playlister.Utilities;

#pragma warning disable 8618

namespace Playlister.Models;

public record UserAccessToken
{
    public required string AccessToken { get; init; }
    public required string? RefreshToken { get; init; }
    public required DateTime Expiration { get; init; }

    public override string ToString() => $"UserAccessToken {this.PrettyPrint()}";
}
