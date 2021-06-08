using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace Playlister.Models
{
    public record UserAccessToken
    {
        [Required]
        public string AccessToken { get; init; }

        [Required]
        public string RefreshToken { get; init; }

        [Required]
        public DateTime Expiration { get; init; }

        [Required]
        public string[] Scopes { get; init; }
    }
}
