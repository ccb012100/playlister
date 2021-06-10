using System.ComponentModel.DataAnnotations;
using Refit;

namespace Playlister.Requests
{
    public record TokenRefreshRequestParams
    {
        public TokenRefreshRequestParams(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        [Required]
        [AliasAs("grant_type")]
        // ReSharper disable once UnusedMember.Global
        public string GrantType { get; init; } = "refresh_token";

        [Required]
        [AliasAs("refresh_token")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string RefreshToken { get; init; }
    }
}
