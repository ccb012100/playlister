using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.Requests
{
    public record CurrentUserRequest : IRequest<PrivateUserObject>;
}
