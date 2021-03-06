using System;
using MediatR;

namespace Playlister.CQRS.Commands
{
    /// <summary>
    /// Request to get the URL for users to sign in to Spotify
    /// </summary>
    public record GetAuthUrlCommand : IRequest<Uri>;
}
