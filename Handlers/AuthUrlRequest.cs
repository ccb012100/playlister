using System;
using MediatR;

namespace Playlister.Handlers
{
    public record AuthUrlRequest : IRequest<Uri>
    {
    }
}
