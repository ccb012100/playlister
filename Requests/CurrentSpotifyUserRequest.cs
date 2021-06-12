using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models.Spotify;

namespace Playlister.Requests
{
    public record CurrentSpotifyUserRequest : IRequest<PrivateUserObject>
    {
        public CurrentSpotifyUserRequest(string accessToken)
        {
            AccessToken = accessToken;
        }

        [Required]
        public string AccessToken { get; }
    }
}
