using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models;

namespace Playlister.Requests
{
    public record SpotifyUserRequest : IRequest<SpotifyUserProfile>
    {
        public SpotifyUserRequest(string accessToken)
        {
            AccessToken = accessToken;
        }

        [Required]
        public string AccessToken { get; }
    }
}
