using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
#pragma warning disable 8618

namespace Playlister.Models
{
    public record Track
    {
        public Album Album { get; init; }

        public IEnumerable<Artist> Artists { get; init; }
        public int DiscNumber { get; init; }
        public int DurationMs { get; init; }
        public string Id { get; init; }
        public string Name { get; init; }
        public int TrackNumber { get; init; }

        /// <summary>
        /// Flatten the track's artists and the track's album's artists into a single collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Artist> GetAllContainedArtists() => Artists.Concat(Album.Artists);

        /// <summary>
        /// Get Track/Artist Id pair for each artist on the track.
        /// </summary>
        /// <returns>Collection of track id, artist id tuples</returns>
        public IEnumerable<(string TrackId, string ArtistId)> GetArtistIdPairings() =>
            Artists.Select(a => (TrackId: Id, ArtistId: a.Id));
    }
}
