using System.ComponentModel.DataAnnotations;

using Refit;

namespace Playlister.Models.SpotifyAccounts;

public record AccessTokenRequestParams {
    /// <summary>
    ///     As defined in the <b>OAuth 2.0</b> specification, this field must contain the value <c>"authorization_code"</c>
    /// </summary>
    [Required]
    [AliasAs( "grant_type" )]
    public string GrantType { get; } = "authorization_code";

    /// <summary>
    ///     The authorization code returned from the initial request to the <b>Spotify Account</b> <c>/authorize</c> endpoint
    /// </summary>
    [Required]
    [AliasAs( "code" )]
    public required string Code { get; init; }

    /// <summary>
    ///     This parameter is used for validation only (there is no actual redirection). The value of this parameter must exactly match the value of
    ///     <c>redirect_uri</c> supplied when requesting the authorization code.
    /// </summary>
    [Required]
    [AliasAs( "redirect_uri" )]
    public required string RedirectUri { get; init; }
}
