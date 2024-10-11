using System.ComponentModel.DataAnnotations;

namespace Playlister.Configuration;

public record SpotifyOptions {
    public const string Spotify = "Spotify";

    [Required] public required string ClientId { get; init; }

    [Required] public required string ClientSecret { get; init; }

    [Required] public required Uri ApiBaseAddress { get; init; }

    [Required] public required Uri AccountsApiBaseAddress { get; init; }

    /// <summary>
    ///     Allowed values:<br /><br />
    ///     - <c>https://localhost:5001/app/home/login</c><br />
    ///     - <c>https://localhost:5001/login</c>
    /// </summary>
    [Required]
    public required Uri CallbackUrl { get; init; }

    /// <summary>
    /// The **SpotifyUri** for the current User.
    /// </summary>
    /// <remarks>
    /// Should be in the format <c>spotify:user:USER_ID</c>, e.g. <c>spotify:user:1234567890</c>
    /// </remarks>
    [Required]
    public required string SpotifyUri { get; init; }
}
