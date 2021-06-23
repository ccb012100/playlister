using System;
using MediatR;

namespace Playlister.CQRS.Requests
{
    /// <summary>
    /// Request to get the URL for users to sign in to Spotify
    /// </summary>
    public record AuthUrlRequest : IRequest<Uri>;
}