// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record ExternalIdObject
    {
        /// <summary>
        /// International Article Number
        /// </summary>
        public string Ean { get; init; }

        /// <summary>
        /// International Standard Recording Code
        /// </summary>
        public string Isrc { get; init; }

        /// <summary>
        /// Universal Product Code
        /// </summary>
        public string Upc { get; init; }
    }
}
