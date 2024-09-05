using Microsoft.AspNetCore.Mvc;

namespace Playlister.Mvc.ViewModels;

[ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
[IgnoreAntiforgeryToken]
public class ErrorViewModel
{
    public required int? ResponseStatusCode { get; init; }

    public string? RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty( RequestId );

    public string? ExceptionMessage { get; init; }
}
