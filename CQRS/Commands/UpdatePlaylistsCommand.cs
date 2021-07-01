using System.Collections.Generic;
using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Requests
{
    public record UpdatePlaylistsCommand(IEnumerable<SimplifiedPlaylistObject> Playlists) : IRequest<Unit>;
}
