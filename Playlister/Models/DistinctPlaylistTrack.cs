using System;
using Playlister.Models;

namespace Playlister.Services
{
    public partial class PlaylistService
    {
        /// <summary>
        /// <see cref="PlaylistTrack"/> properties that represent a unique PlaylistTrack.
        /// </summary>
        /// <param name="PlaylistId"></param>
        /// <param name="TrackId"></param>
        /// <param name="AddedAt"></param>
        public record DistinctPlaylistTrack(string PlaylistId, string TrackId, DateTime AddedAt);
    }
}
