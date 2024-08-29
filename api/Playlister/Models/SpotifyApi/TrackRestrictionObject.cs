#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi;

public record TrackRestrictionObject : IRestrictionObject
{
    public string Reason { get; init; }
}
