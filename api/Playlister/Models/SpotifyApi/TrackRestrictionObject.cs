namespace Playlister.Models.SpotifyApi;

public record TrackRestrictionObject : IRestrictionObject {
    public required string Reason { get; init; }
}
