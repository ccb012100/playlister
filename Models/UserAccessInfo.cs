using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace Playlister.Models
{
    public record UserAccessInfo
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public DateTime Expiration { get; init; }
        public string[] Scopes { get; init; }
    }
}
