using System.Collections.Generic;
using MediatR;
using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Requests
{
    public record UpsertPlaylistsRequest : IRequest<Unit>
    {
        public UpsertPlaylistsRequest(IEnumerable<SimplifiedPlaylistObject> playlists)
        {
            Playlists = playlists;
        }

        public IEnumerable<SimplifiedPlaylistObject> Playlists { get; init; }
    }
}
