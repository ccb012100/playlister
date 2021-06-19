using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Requests
{
    public record CurrentUserRequest : IRequest<PrivateUserObject>;
}
