using System;

namespace Playlister.Models.Spotify
{
    public record ImageObject
    {
        /// <summary>
        /// The image height in pixels. If unknown: `null` or not returned.
        /// </summary>
        public int? Height { get; init; }

        /// <summary>
        /// The source URL of the image.
        /// </summary>
        public Uri Url { get; init; }

        /// <summary>
        /// The image width in pixels. If unknown: `null` or not returned.
        /// </summary>
        public int? Width { get; init; }
    }
}
