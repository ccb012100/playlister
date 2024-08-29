using System.Text.Json.Serialization;

#pragma warning disable 8618

namespace Playlister.Models.SpotifyAccounts
{
    public record SpotifyAccessToken
    {
        private const string BearerType = "Bearer";
        private readonly string _tokenType;

        [JsonPropertyName("access_token")] public string AccessToken { get; init; }

        /// <summary>
        ///     How the access token may be used: always <c>Bearer</c>.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType
        {
            get => _tokenType;

            init
            {
                if (value != BearerType)
                {
                    throw new ArgumentException($"Invalid TokenType `{value}`. Value should always be `{BearerType}`");
                }

                _tokenType = value;
            }
        }

        /// <summary>
        ///     A space-separated list of scopes which have been granted for this <c>access_token</c>
        /// </summary>
        public string Scope { get; init; }

        /// <summary>
        ///     The time period (in seconds) for which the access token is valid.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        /// <summary>
        ///     A token that can be sent to the Spotify Accounts service in place of an authorization code.
        ///     (When the access code expires, send a <c>POST</c> request to the Accounts service <c>/api/token</c> endpoint, but use this code in place of an
        ///     authorization code. A new access token will be returned. A new refresh token might be returned too.)
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; }

        /// <remarks>make non-nullable</remarks>
        public UserAccessToken ToUserAccessToken() =>
            new() { AccessToken = AccessToken, Expiration = DateTime.Now.AddSeconds(ExpiresIn), RefreshToken = RefreshToken };
    }
}
