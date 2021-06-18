#pragma warning disable 8618
namespace Playlister.Models.SpotifyApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record AlbumRestrictionObject : IRestrictionObject
    {
        public string Reason { get; init; }
    }
}
