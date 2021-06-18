using System;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record PagingObject<T>
    {
        /// <summary>
        /// A link to the Web API endpoint returning the full result of the request
        /// </summary>
        public Uri Href { get; init; }

        /// <summary>
        /// The requested data.
        /// </summary>
        // ReSharper disable once CollectionNeverUpdated.Global
        public ICollection<T> Items { get; init; }

        /// <summary>
        /// The maximum number of items in the response (as set in the query or by default).
        /// </summary>
        public int Limit { get; init; }

        /// <summary>
        /// URL to the next page of items. (<c>null</c> if none)
        /// </summary>
        public Uri? Next { get; init; }

        /// <summary>
        /// The offset of the items returned (as set in the query or by default)
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// URL to the previous page of items. (<c>null</c> if none)
        /// </summary>
        public Uri? Previous { get; init; }

        /// <summary>
        /// The total number of items available to return.
        /// </summary>
        public int Total { get; init; }
    }
}
