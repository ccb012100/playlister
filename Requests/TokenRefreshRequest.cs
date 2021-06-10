using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models;

namespace Playlister.Requests
{
    public record TokenRefreshRequest : IRequest<UserAccessToken>
    {
        // ReSharper disable once UnusedMember.Global
        public TokenRefreshRequest(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        [Required]
        // The refresh token returned from the authorization code exchange.
        public string RefreshToken { get; }
    }
}
