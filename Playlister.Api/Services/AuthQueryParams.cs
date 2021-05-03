using System;

namespace Playlister.Api.Services
{
    public record AuthQueryParams
    {
        public string ClientId { get; init; }
        public Uri RedirectUri { get; init; }
        public string State { get; init; }
        public string? Scope { get; init; }
        public bool ShowDialog { get; init; }
        public string ResponseType { get; init; } = "code";
    }
}