using Playlister.Utilities;

namespace Playlister.Models;

/// <summary>
///     Represents a <b>Spotify</b> Access Token
/// </summary>
public record AuthenticationToken {
    /// <summary>
    ///     Credentials and permissions that can be used to access a given resource (e.g. artists, albums or tracks) or a user's data.
    /// </summary>
    /// <remarks>
    ///     Lifespan = 1 hour
    /// </remarks>
    public required string AccessToken { get; init; }

    /// <summary>
    ///     A security credential that allows the application to obtain new access tokens
    ///     without requiring the user to reauthorize the application.
    /// </summary>
    /// <remarks>
    ///     Lifespan = 1 hour
    /// </remarks>
    public required string? RefreshToken { get; init; }

    /// <summary>
    ///     Expiration time as a UTC DateTime
    /// </summary>
    public required DateTime ExpirationUtc { get; init; }

    public override string ToString( ) {
        return $"UserAccessToken {this.PrettyPrint( )}";
    }
}
