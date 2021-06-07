using System;

namespace Playlister
{
    public struct SpotifyAccessToken
    {
        private const string BearerType = "Bearer";
        private string _tokenType;

        public string AccessToken { get; set; }

        // How the access token may be used: always “Bearer”.
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
        public string Scope { get; set; }

        // The time period (in seconds) for which the access token is valid.
        public int ExpiresIn { get; set; }

        /*
         * A token that can be sent to the Spotify Accounts service in place of an authorization code.
         * (When the access code expires, send a POST request to the Accounts service /api/token endpoint,
         * but use this code in place of an authorization code. A new access token will be returned.
         * A new refresh token might be returned too.)
         */
        public string RefreshToken { get; set; }
    }
}
