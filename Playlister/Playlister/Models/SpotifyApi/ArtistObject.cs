using System.Collections.Generic;

#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    public record ArtistObject : SimplifiedArtistObject
    {
        /// <summary>
        /// Information about the followers of the artist.
        /// </summary>
        public FollowersObject Followers { get; init; }

        /// <summary>
        /// Images of the artist in various sizes, widest first.
        /// </summary>
        public ICollection<ImageObject> Images { get; init; }

        /// <summary>
        /// The popularity of the artist.
        /// The value will be between <c>0</c> and <c>100</c>, with <c>100</c> being the most popular.
        /// The artist’s popularity is calculated from the popularity of all the artist’s tracks.
        /// </summary>
        public int Popularity { get; init; }
    }
}
