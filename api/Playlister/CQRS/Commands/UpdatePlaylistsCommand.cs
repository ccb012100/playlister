using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Commands
{
    public record UpdatePlaylistsCommand(string AccessToken, IEnumerable<SimplifiedPlaylistObject> Playlists)
        : IRequest<Unit>;
}
