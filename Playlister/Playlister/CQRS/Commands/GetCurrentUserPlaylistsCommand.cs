using System.Collections.Generic;
using MediatR;
using Playlister.Models;


namespace Playlister.CQRS.Commands
{
    public record GetCurrentUserPlaylistsCommand(string AccessToken)
        : IRequest<IEnumerable<Playlist>>;
}
