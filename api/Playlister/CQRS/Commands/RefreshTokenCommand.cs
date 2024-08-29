using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models;
using Refit;

namespace Playlister.CQRS.Commands
{
    /// <summary>
    ///     Request to refresh Spotify Access Token
    /// </summary>
    public record RefreshTokenCommand : IRequest<UserAccessToken>
    {
        public RefreshTokenCommand(string refreshToken) => RefreshToken = refreshToken;

        [Required]
        // The refresh token returned from the authorization code exchange.
        public string RefreshToken { get; }

        public record BodyParams
        {
            public BodyParams(string refreshToken) => RefreshToken = refreshToken;

            [Required][AliasAs("grant_type")] public string GrantType { get; init; } = "refresh_token";

            [Required][AliasAs("refresh_token")] public string RefreshToken { get; init; }
        }
    }
}
