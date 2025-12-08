using System.ComponentModel.DataAnnotations;

namespace Playlister.Configuration;

public record SpotifyOptions {
    public const string Spotify = "Spotify";

    /// <summary>
    /// App Client ID used for requesting an auth token from Spotify (found in the Spotify Developer Dashboard).
    /// </summary>
    [Required]
    public required string ClientId { get; init; }

    /// <summary>
    /// App Client Secret used for requesting an auth token from Spotify (found in the Spotify Developer Dashboard).
    /// </summary>
    [Required]
    public required string ClientSecret { get; init; }

    /// <summary>
    /// Base URL for the Spotify Web API.
    /// </summary>
    [Required]
    public required Uri ApiBaseAddress { get; init; }

    /// <summary>
    /// Base URL for the Spotify OAuth 2.0 Service API.
    /// </summary>
    [Required]
    public required Uri AccountsApiBaseAddress { get; init; }

    /// <summary>
    /// Redirect URLs for the app used in Spotify's auth flow (configured in the Spotify Developer Dashboard).
    /// </summary>
    /// <remarks>
    /// <list type="table">
    ///     <listheader>
    ///         <description>
    ///             Allowed URLs
    ///         </description>
    ///     </listheader>
    ///     <item>
    ///         <description>
    ///             <c>https://127.0.0.1:5001/app/home/login</c>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <c>https://127.0.0.1:5001/login</c>
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    [Required]
    public required Uri CallbackUrl { get; init; }

    /// <summary>
    /// The <b>SpotifyUri</b> for the current User.
    /// </summary>
    /// <remarks>
    /// Should be in the format <c>spotify:user:USER_ID</c>, e.g. <c>spotify:jdoe:1234567890</c>
    /// </remarks>
    [Required]
    public required string SpotifyUri { get; init; }

    /// <summary>
    /// Prefix used on playlist used to keep the "now playing" Queue persistent across listening sessions.
    /// </summary>
    /// <remarks>
    /// Sometimes, and seemingly randomly, Spotify will wipe out the Queue, so I created a playlist to house it when I'm checking out new music and
    /// have queued up a lot of stuff. I don't want to sync it locally, though. It's now expanded to multiple queues.
    /// </remarks>
    [Required]
    public required string PersistentQueueNamePrefix { get; init; }
}
