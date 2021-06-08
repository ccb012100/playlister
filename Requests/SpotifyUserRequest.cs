using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Playlister.Requests
{
    public record SpotifyUserRequest : IRequest<string>
    {
        public SpotifyUserRequest(string accessToken)
        {
            AccessToken = accessToken;
        }

        [Required]
        public string AccessToken { get; }
    }
}
