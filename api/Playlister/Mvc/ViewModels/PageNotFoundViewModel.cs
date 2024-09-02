using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Playlister.Mvc.ViewModels;

[ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
public class PageNotFoundViewModel : PageModel
{
    public string? OriginalPathAndQuery { get; set; }

    public void OnGet( int statusCode )
    {
        IStatusCodeReExecuteFeature? statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        if (statusCodeReExecuteFeature is null)
        {
            return;
        }

        OriginalPathAndQuery
            = $"{statusCodeReExecuteFeature.OriginalPathBase}{statusCodeReExecuteFeature.OriginalPath}{statusCodeReExecuteFeature.OriginalQueryString}";
    }
}
