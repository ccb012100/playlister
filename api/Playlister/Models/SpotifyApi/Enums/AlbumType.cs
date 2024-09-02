using System.Runtime.Serialization;

namespace Playlister.Models.SpotifyApi.Enums;

public enum AlbumType
{
    [EnumMember( Value = "album" )] Album,
    [EnumMember( Value = "single" )] Single,
    [EnumMember( Value = "compilation" )] Compilation
}
