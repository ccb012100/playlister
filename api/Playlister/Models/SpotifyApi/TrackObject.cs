#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi;

public record TrackObject : SimplifiedTrackObject
{
    /// <summary>
    ///     The album on which the track appears. The album object includes a link in <c>href</c> to full information about the album.
    /// </summary>
    public SimplifiedAlbumObject Album { get; init; }

    /// <summary>
    ///     The artists who performed the track. Each artist object includes a link in <c>href</c> to more detailed information about the artist.
    /// </summary>
    public new IEnumerable<ArtistObject> Artists { get; init; }

    /// <summary>
    ///     Known external IDs for the track.
    /// </summary>
    public ExternalIdObject ExternalIds { get; init; }

    /// <summary>
    ///     The popularity of the track. The value will be between <c>0</c> and <c>100</c>, with <c>100</c> being the most popular.
    ///     The popularity is calculated by algorithm and is based, in the most part, on the total number of plays the track has had and how recent those
    ///     plays are.
    ///     Generally speaking, songs that are being played a lot now will have a higher popularity than songs that were played a lot in the past.
    ///     Duplicate tracks (e.g. the same track from a single and an album) are rated independently.
    ///     Artist and album popularity is derived mathematically from track popularity.
    ///     Note that the popularity value may lag actual popularity by a few days: the value is not updated in real time.
    /// </summary>
    public int Popularity { get; init; }
}
