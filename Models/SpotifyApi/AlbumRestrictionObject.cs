namespace Playlister.Models.SpotifyApi
{
    public record AlbumRestrictionObject : IRestrictionObject
    {
        public string Reason { get; init; }
    }
}
