using System.ComponentModel.DataAnnotations;
using MediatR;

#pragma warning disable 8618

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Playlister.CQRS.Requests
{
    /// <summary>
    /// Request to Update the Playlist data stored in the database
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public record UpdatePlaylistItemsRequest : IRequest<Unit>
    {
        public UpdatePlaylistItemsRequest(string playlistId)
        {
            PlaylistId = playlistId;
        }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string PlaylistId { get; }

        /// <summary>
        /// The index of the first playlist to return.
        /// Default: <c>0</c> (the first object). Maximum offset: <c>100.000</c>.
        /// Use with limit to get the next set of playlists.
        /// </summary>
        [Range(0, 100_000)]
        public int Offset { get; init; } = 0;

        /// <summary>
        /// The maximum number of playlists to return.
        /// Default: <c>50</c>. Minimum: <c>1</c>. Maximum: <c>50</c>.
        /// Note: the Default on Spotify API is 50.
        /// </summary>
        [Range(0, 50)]
        public int Limit { get; init; } = 50;
    }
}
