using System.Text.Json.Serialization;

namespace Playlister.Models.SpotifyApi.Enums;

[JsonConverter( typeof(JsonStringEnumConverter) )]
public enum AlbumType
{
    Album,
    Single,
    Compilation
}
