using MediatR;

namespace Playlister.CQRS.Commands
{
    public record UpdateCurrentUserPlaylistsCommand : IRequest<Unit>;
}
