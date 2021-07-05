using System.ComponentModel.DataAnnotations;
using MediatR;

#pragma warning disable 8618

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Playlister.CQRS.Commands
{
    /// <summary>
    /// Request to Update the Playlist data stored in the database
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public record UpdatePlaylistCommand : IRequest<Unit>
    {
        public UpdatePlaylistCommand(string accessToken, string playlistId)
        {
            AccessToken = accessToken;
            PlaylistId = playlistId;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string AccessToken { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string PlaylistId { get; }

        /// <summary>
        /// The index of the first track to return.
        /// Default: <c>0</c> (the first object). Maximum offset: <c>100.000</c>.
        /// Use with limit to get the next set of playlists.
        /// </summary>
        [Range(0, 100_000)]
        public int Offset { get; init; } = 0;

        /// <summary>
        /// The maximum number of tracks to return.
        /// Default: <c>50</c>. Minimum: <c>1</c>. Maximum: <c>50</c>.
        /// Note: the Default on Spotify API is 50.
        /// </summary>
        [Range(0, 50)]
        public int Limit { get; init; } = 50;
    }
}
