using System.ComponentModel.DataAnnotations;
using MediatR;
using Playlister.Models.SpotifyApi;

// ReSharper disable UnusedMember.Global

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Playlister.Requests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record CurrentUserPlaylistsRequest : IRequest<PagingObject<SimplifiedPlaylistObject>>
    {
        public CurrentUserPlaylistsRequest(int? offset, int? limit)
        {
            if (offset.HasValue) Offset = offset.Value;

            if (limit.HasValue) Limit = limit.Value;
        }

        /// <summary>
        /// The maximum number of playlists to return.
        /// Default: <c>50</c>. Minimum: <c>1</c>. Maximum: <c>50</c>.
        /// Note: Spotify API Default is <c>20</c>
        /// </summary>
        [Range(1, 50)]
        public int Limit { get; init; } = 50;

        /// <summary>
        /// The index of the first playlist to return.
        /// Default: <c>0</c> (the first object).
        /// Maximum offset: <c>100.000</c>.
        /// Use with <c>limit</c> to get the next set of playlists.
        /// </summary>
        [Range(0, 100_000)]
        public int Offset { get; init; }
    }
}
