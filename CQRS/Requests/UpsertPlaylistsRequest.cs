using System.Collections.Generic;
using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Requests
{
    public record UpsertPlaylistsRequest(IEnumerable<SimplifiedPlaylistObject> Playlists) : IRequest<Unit>;
}
