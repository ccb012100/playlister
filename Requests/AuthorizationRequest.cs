using MediatR;

namespace Playlister.Requests
{
    public record AuthorizationRequest : IRequest<string>
    {
    }
}