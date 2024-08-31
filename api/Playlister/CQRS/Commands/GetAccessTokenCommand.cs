using System.ComponentModel.DataAnnotations;
using Refit;

namespace Playlister.CQRS.Commands;

/// <summary>
///     Request to get an Access Token from Spotify
/// </summary>
public record GetAccessTokenCommand : Command
{
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

    public record BodyParams
    {
        // As defined in the OAuth 2.0 specification, this field must contain the value "authorization_code"
        [Required] [AliasAs("grant_type")] public string GrantType { get; } = "authorization_code";

        // The authorization code returned from the initial request to the Spotify Account /authorize endpoint
        [Required] [AliasAs("code")] public required string Code { get; init; }

        /*
         * This parameter is used for validation only (there is no actual redirection).
         * The value of this parameter must exactly match the value of redirect_uri supplied when requesting the authorization code.
         */
        [Required] [AliasAs("redirect_uri")] public required string RedirectUri { get; init; }

        [Required] [AliasAs("client_id")] public required string ClientId { get; init; }

        [Required] [AliasAs("client_secret")] public required string ClientSecret { get; init; }
    }
}
