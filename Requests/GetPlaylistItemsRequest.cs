using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models.SpotifyApi;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Playlister.Requests
{
    /// <summary>
    /// Request to get the full details of the items of a playlist.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public record GetPlaylistItemsRequest : IRequest<PagingObject<TrackObject>>
    {
        public GetPlaylistItemsRequest(int? limit, int? offset, string? fields)
        {
            Limit = limit;
            Offset = offset;
            Fields = fields;
        }

        /// <summary>
        /// The maximum number of playlists to return.
        /// Default: <c>100</c>. Minimum: <c>1</c>. Maximum: <c>100</c>.
        /// </summary>
        [Range(1, 50)]
        public int? Limit { get; init; }

        /// <summary>
        /// The index of the first playlist to return.
        /// Default: <c>0</c> (the first object).
        /// Use with <c>limit</c> to get the next set of playlists.
        /// </summary>
        public int? Offset { get; init; }

        /// <summary>
        /// Filters for the query: a comma-separated list of the fields to return. If omitted, all fields are returned.
        /// </summary>
        public string? Fields { get; init; }
    }
}
