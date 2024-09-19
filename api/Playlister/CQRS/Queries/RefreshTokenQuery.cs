namespace Playlister.CQRS.Queries;

/// <summary>
///     Request to refresh Spotify Access Token
/// </summary>
public record RefreshTokenQuery {
    public RefreshTokenQuery( string refreshToken ) {
        RefreshToken = refreshToken;
    }

    /// The refresh token returned from the authorization code exchange.
    public string RefreshToken { get; }
}
