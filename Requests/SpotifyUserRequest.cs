using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models.Spotify;

namespace Playlister.Requests
{
    public record SpotifyUserRequest : IRequest<PublicUserObject>
    {
        public SpotifyUserRequest(string accessToken)
        {
            AccessToken = accessToken;
        }

        [Required]
        public string AccessToken { get; }
    }
}
