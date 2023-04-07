using System;

// ReSharper disable UnusedMember.Global

#pragma warning disable 8618

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Playlister.CQRS.Commands
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
