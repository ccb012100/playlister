using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Requests
{
    public record CurrentUserCommand : IRequest<PrivateUserObject>;
}
