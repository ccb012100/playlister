namespace Playlister.Models.Spotify
{
    public record ExplicitContentSettingsObject
    {
        /// <summary>
        /// When `true`, indicates that explicit content should not be played.
        /// </summary>
        public bool FilterEnabled { get; set; }

        /// <summary>
        /// When `true`, indicates that the explicit content setting is locked and can’t be changed by the user.
        /// </summary>
        public bool FilterLocked { get; set; }
    }
}
