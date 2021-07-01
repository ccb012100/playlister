using MediatR;

namespace Playlister.CQRS.Commands
{
    public record CurrentUserUpdatePlaylistsCommand : IRequest<Unit>;
}
