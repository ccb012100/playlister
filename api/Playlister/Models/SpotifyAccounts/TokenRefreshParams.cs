using System.ComponentModel.DataAnnotations;
using Refit;

namespace Playlister.Models.SpotifyAccounts;

public record TokenRefreshParams
{
    public TokenRefreshParams( string refreshToken )
    {
        RefreshToken = refreshToken;
    }

    [Required][AliasAs( "grant_type" )] public string GrantType { get; init; } = "refresh_token";

    [Required]
    [AliasAs( "refresh_token" )]
    public string RefreshToken { get; init; }
}
