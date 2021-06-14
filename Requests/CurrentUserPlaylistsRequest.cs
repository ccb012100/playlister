using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models.SpotifyApi;
using Refit;

namespace Playlister.Requests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record CurrentUserPlaylistsRequest : IRequest<PagingObject<PlaylistObject>>
    {
        public CurrentUserPlaylistsRequest(int? offset, int? limit)
        {
            Offset = offset;
            Limit = limit;
        }

        /// <summary>
        /// The maximum number of playlists to return. Default: 20. Minimum: 1. Maximum: 50.
        /// </summary>
        [Range(1, 50)]
        [AliasAs("limit")]
        public int? Limit { get; init; }

        /// <summary>
        /// The index of the first playlist to return. Default: 0 (the first object). Maximum offset: 100.000. Use with limit to get the next set of playlists.
        /// </summary>
        [AliasAs("offset")]
        public int? Offset { get; init; }
    }
}
