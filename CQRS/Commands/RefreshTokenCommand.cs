using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models;
using Refit;

namespace Playlister.CQRS.Commands
{
    /// <summary>
    /// Request to refresh Spotify Access Token
    /// </summary>
    public record RefreshTokenCommand : IRequest<UserAccessToken>
    {
        // ReSharper disable once UnusedMember.Global
        public RefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        [Required]
        // The refresh token returned from the authorization code exchange.
        public string RefreshToken { get; }

        // ReSharper disable once ClassNeverInstantiated.Global
        public record BodyParams
        {
            public BodyParams(string refreshToken)
            {
                RefreshToken = refreshToken;
            }

            [Required, AliasAs("grant_type")]
            // ReSharper disable once UnusedMember.Global
            public string GrantType { get; init; } = "refresh_token";

            [Required, AliasAs("refresh_token")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            // ReSharper disable once MemberCanBePrivate.Global
            public string RefreshToken { get; init; }
        }
    }
}
