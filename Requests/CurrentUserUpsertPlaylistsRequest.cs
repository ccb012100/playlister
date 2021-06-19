using MediatR;

namespace Playlister.Requests
{
    public record CurrentUserUpsertPlaylistsRequest : IRequest<Unit>;
}
