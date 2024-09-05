using Microsoft.AspNetCore.Mvc;

namespace Playlister.Mvc.ViewModels;

[ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
public record PageNotFoundViewModel( string OriginalPathAndQuery );
