using MediatR;

namespace Playlister.Api.Handlers
{
    public record AuthorizationRequest: IRequest<string>
    {
    }
}
