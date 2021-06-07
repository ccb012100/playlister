using System;
using MediatR;

namespace Playlister.Requests
{
    public record AuthUrlRequest : IRequest<Uri>
    {
    }
}