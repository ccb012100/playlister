namespace Playlister.Models.SpotifyApi;

public record ExplicitContentSettingsObject {
    /// <summary>
    ///     When <c>true</c>, indicates that explicit content should not be played.
    /// </summary>
    public bool FilterEnabled { get; set; }

    /// <summary>
    ///     When <c>true</c>, indicates that the explicit content setting is locked and can’t be changed by the user.
    /// </summary>
    public bool FilterLocked { get; set; }
}
