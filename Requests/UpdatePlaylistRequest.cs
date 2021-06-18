using MediatR;

namespace Playlister.Requests
{
    /// <summary>
    /// Request to Update the Playlist data stored in the database
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public record UpdatePlaylistRequest : IRequest<Unit>
    {
        public UpdatePlaylistRequest(string playlistId)
        {
            PlaylistId = playlistId;
        }

        public string PlaylistId { get; init; }

        /// <summary>
        /// The index of the first playlist to return.
        /// Default: <c>0</c> (the first object). Maximum offset: <c>100.000</c>.
        /// Use with limit to get the next set of playlists.
        /// </summary>
        public int? Offset { get; init; }

        /// <summary>
        /// The maximum number of playlists to return.
        /// Default: <c>20</c>. Minimum: <c>1</c>. Maximum: <c>50</c>.
        /// </summary>
        public int? Limit { get; init; }
    }
}
