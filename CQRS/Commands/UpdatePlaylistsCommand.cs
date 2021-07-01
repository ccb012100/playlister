using System.Collections.Generic;
using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Commands
{
    public record UpdatePlaylistsCommand(IEnumerable<SimplifiedPlaylistObject> Playlists) : IRequest<Unit>;
}
