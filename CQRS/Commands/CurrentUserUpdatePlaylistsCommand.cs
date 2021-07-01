using MediatR;

namespace Playlister.CQRS.Requests
{
    public record CurrentUserUpdatePlaylistsCommand : IRequest<Unit>;
}
