namespace Playlister.Models.SpotifyApi
{
    public record TrackRestrictionObject : IRestrictionObject
    {
        public string Reason { get; init; }
    }
}
