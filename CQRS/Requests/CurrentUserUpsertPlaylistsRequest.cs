using MediatR;

namespace Playlister.CQRS.Requests
{
    public record CurrentUserUpsertPlaylistsRequest : IRequest<Unit>;
}
