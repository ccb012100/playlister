#pragma warning disable 8618

namespace Playlister.Models
{
    public record UserAccessToken
    {
        public string AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public DateTime Expiration { get; init; }
    }
}
