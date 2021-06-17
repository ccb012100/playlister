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
    }
}
