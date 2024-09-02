namespace Playlister.Models.SpotifyApi;

public record AlbumRestrictionObject : IRestrictionObject
{
    public required string Reason { get; init; }
}
