using MediatR;

namespace Playlister.Handlers
{
    public record AuthorizationRequest: IRequest<string>
    {
    }
}
