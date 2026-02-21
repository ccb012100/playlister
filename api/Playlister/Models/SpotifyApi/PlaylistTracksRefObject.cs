namespace Playlister.Models.SpotifyApi;

public record PlaylistTracksRefObject
{
    public required Uri Href { get; init; }
    public int Total { get; init; }
}
