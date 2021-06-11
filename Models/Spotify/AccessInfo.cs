using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable 8618

namespace Playlister.Models.Spotify
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record AccessInfo
    {
        private const string BearerType = "Bearer";
        private string _tokenType;

        [Required]
        public string AccessToken { get; init; }

        // How the access token may be used: always “Bearer”.
        [Required]
        public string TokenType
        {
            get => _tokenType;
            set
            {
                if (value != BearerType)
                {
                    throw new ArgumentException($"Found `{value}`. This should always be type `{BearerType}`");
                }

                _tokenType = value;
            }
        }

        // A space-separated list of scopes which have been granted for this `access_token`
        [Required]
        public string Scope { get; init; }

        // The time period (in seconds) for which the access token is valid.
        [Required]
        public int ExpiresIn { get; init; }

        /*
         * A token that can be sent to the Spotify Accounts service in place of an authorization code.
         * (When the access code expires, send a POST request to the Accounts service /api/token endpoint,
         * but use this code in place of an authorization code. A new access token will be returned.
         * A new refresh token might be returned too.)
         */
        [Required]
        public string RefreshToken { get; init; }
    }
}
