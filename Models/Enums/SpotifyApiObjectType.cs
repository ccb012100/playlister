using System.Runtime.Serialization;

namespace Playlister.Models.Enums
{
    public enum SpotifyApiObjectType
    {
        [EnumMember(Value = "album")] Album,
        [EnumMember(Value = "playlist")] Playlist,
        [EnumMember(Value = "track")] Track,
        [EnumMember(Value = "user")] User
    }
}
