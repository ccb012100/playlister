using MediatR;

namespace Playlister.Requests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record AuthorizationRequest : IRequest<string>
    {
    }
}
