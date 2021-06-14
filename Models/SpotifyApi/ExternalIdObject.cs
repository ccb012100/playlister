namespace Playlister.Models.SpotifyApi
{
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
