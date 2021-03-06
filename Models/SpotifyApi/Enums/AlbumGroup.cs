using System.Runtime.Serialization;

namespace Playlister.Models.SpotifyApi.Enums
{
    public enum AlbumGroup
    {
        [EnumMember(Value = "album")] Album,
        [EnumMember(Value = "single")] Single,
        [EnumMember(Value = "compilation")] Compilation,
        [EnumMember(Value = "appears_on")] AppearsOn
    }
}
