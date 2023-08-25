using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Commands
{
    public record GetCurrentUserCommand(string AccessToken) : IRequest<PrivateUserObject>;
}
