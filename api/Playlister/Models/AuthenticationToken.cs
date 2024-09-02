using Playlister.Utilities;

namespace Playlister.Models;

/// <summary>
///     Represents a <b>Spotify</b> Access Token
/// </summary>
public record AuthenticationToken
{
    public required string AccessToken { get; init; }
    public required string? RefreshToken { get; init; }
    public required DateTime Expiration { get; init; }

    public override string ToString()
    {
        return $"UserAccessToken {this.PrettyPrint()}";
    }
}
