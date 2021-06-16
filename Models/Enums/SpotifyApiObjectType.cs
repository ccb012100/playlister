using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Playlister.Models.Enums
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public enum SpotifyApiObjectType
    {
        [EnumMember(Value = "album")] Album,
        [EnumMember(Value = "artist")] Artist,
        [EnumMember(Value = "playlist")] Playlist,
        [EnumMember(Value = "track")] Track,
        [EnumMember(Value = "user")] User
    }
}
