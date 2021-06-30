using MediatR;

namespace Playlister.CQRS.Requests
{
    public record CurrentUserUpdatePlaylistsRequest : IRequest<Unit>;
}
