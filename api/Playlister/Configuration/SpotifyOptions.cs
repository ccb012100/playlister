using System.ComponentModel.DataAnnotations;

#pragma warning disable 8618

namespace Playlister.Configuration
{
    public record SpotifyOptions
    {
        public const string Spotify = "Spotify";

        private static readonly Uri[] s_validCallbackUrls
            = { new("https://localhost:5001/app/home/login"), new("https://localhost:5001/login") };

        private readonly Uri _callback;

        [Required] public required string ClientId { get; init; }

        [Required] public required string ClientSecret { get; init; }
        [Required] public required Uri ApiBaseAddress { get; init; }
        [Required] public required Uri AccountsApiBaseAddress { get; init; }

        /// <summary>
        /// Allowed values:<br/><br/>
        /// - <c>https://localhost:5001/app/home/login</c><br/>
        /// - <c>https://localhost:5001/login</c>
        /// </summary>
        [Required]
        public required Uri CallbackUrl
        {
            get => _callback;

            init
            {
                _callback = s_validCallbackUrls.Contains(value)
                    ? value
                    : throw new ArgumentException($"Invalid CallbackUrl: {value}");
            }
        }
    }
}
