namespace Playlister.Models.SpotifyAccounts;

/// <summary>
///     Represents the values supplied in successful redirect back to the application from <b>Spotify's</b> authorization dialog
/// </summary>
public record AuthorizationResult {
    /// <summary>
    ///     An authorization code that can be exchanged for an access token
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    ///     The value of the state parameter supplied in the request
    /// </summary>
    public required string State { get; init; }
}
