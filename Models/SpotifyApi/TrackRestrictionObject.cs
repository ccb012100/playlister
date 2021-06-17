// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

#pragma warning disable 8618

namespace Playlister.Models.SpotifyApi
{
    public record TrackRestrictionObject : IRestrictionObject
    {
        public string Reason { get; init; }
    }
}
