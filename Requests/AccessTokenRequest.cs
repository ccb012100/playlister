using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable 8618

namespace Playlister.Requests
{
    public record AccessTokenRequest : IRequest<UserAccessToken>
    {
        [Required]
        // The authorization code returned from the initial request to the Spotify Account /authorize endpoint.
        public string Code { get; init; }

        [Required]
        // The value of the `state` parameter supplied in the request to the Spotify Account /authorize endpoint.
        public string State { get; init; }
    }
}
