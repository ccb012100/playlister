using System.ComponentModel.DataAnnotations;

namespace Playlister.CQRS.Queries;

/// <summary>
///     Request to get an Access Token from Spotify
/// </summary>
public record GetAccessTokenQuery {
    /// <summary>
    ///     The authorization code returned from the initial request to the Spotify Account /authorize endpoint.
    /// </summary>
    [Required]
    public required string Code { get; init; }

    /// <summary>
    ///     The value of the `state` parameter supplied in the request to the Spotify Account /authorize endpoint.
    /// </summary>
    [Required]
    public required string State { get; init; }
}
