using System.ComponentModel.DataAnnotations;
using Refit;

namespace Playlister.CQRS.Queries;

/// <summary>
///     Request to refresh Spotify Access Token
/// </summary>
public record RefreshTokenQuery
{
    public RefreshTokenQuery( string refreshToken )
    {
        RefreshToken = refreshToken;
    }

    [Required]
    // The refresh token returned from the authorization code exchange.
    public string RefreshToken { get; }

    public record BodyParams
    {
        public BodyParams( string refreshToken )
        {
            RefreshToken = refreshToken;
        }

        [Required] [AliasAs( "grant_type" )] public string GrantType { get; init; } = "refresh_token";

        [Required]
        [AliasAs( "refresh_token" )]
        public string RefreshToken { get; init; }
    }
}
