using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.Mvc.ViewModels;

namespace Playlister.Mvc.Controllers;

[ValidateTokenCookie]
public class SyncController : Controller
{
    public const string Name = "Sync";

    public Task<IActionResult> Index()
    {
        return Task.FromResult<IActionResult>( View( new SyncViewModel() ) );
    }
}
