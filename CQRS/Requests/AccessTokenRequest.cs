using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models;
using Refit;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable 8618

namespace Playlister.CQRS.Requests
{
    /// <summary>
    /// Request to get an Access Token from Spotify
    /// </summary>
    public record AccessTokenRequest : IRequest<UserAccessToken>
    {
        /// <summary>
        /// The authorization code returned from the initial request to the Spotify Account /authorize endpoint.
        /// </summary>
        [Required]
        public string Code { get; init; }

        /// <summary>
        /// The value of the `state` parameter supplied in the request to the Spotify Account /authorize endpoint.
        /// </summary>
        [Required]
        public string State { get; init; }

        public record BodyParams
        {
            // As defined in the OAuth 2.0 specification, this field must contain the value
            [Required, AliasAs("grant_type")]
            public string GrantType { get; init; } = "authorization_code";

            // The authorization code returned from the initial request to the Spotify Account /authorize endpoint
            [Required, AliasAs("code")]
            public string Code { get; init; }

            /*
             * This parameter is used for validation only (there is no actual redirection).
             * The value of this parameter must exactly match the value of redirect_uri supplied when requesting the authorization code.
             */
            [Required, AliasAs("redirect_uri")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public string RedirectUri { get; init; }

            [Required, AliasAs("client_id")]
            public string ClientId { get; init; }

            [Required, AliasAs("client_secret")]
            public string ClientSecret { get; init; }
        }
    }
}
