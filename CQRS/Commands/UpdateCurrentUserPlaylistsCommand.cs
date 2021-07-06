using MediatR;

namespace Playlister.CQRS.Commands
{
    public record UpdateCurrentUserPlaylistsCommand(string AccessToken) : IRequest<int>;
}
