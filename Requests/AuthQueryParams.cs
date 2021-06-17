using System;

// ReSharper disable UnusedMember.Global

#pragma warning disable 8618

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Playlister.Requests
{
    // ReSharper disable once UnusedType.Global
    public record AuthQueryParams
    {
        public string ClientId { get; init; }
        public Uri RedirectUri { get; init; }
        public string State { get; init; }
        public string? Scope { get; init; }
        public bool ShowDialog { get; init; }

        // ReSharper disable once UnusedMember.Global
        public string ResponseType { get; init; } = "code";
    }
}
