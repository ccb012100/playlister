#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    public record AlbumRestrictionObject : IRestrictionObject
    {
        public string Reason { get; init; }
    }
}
