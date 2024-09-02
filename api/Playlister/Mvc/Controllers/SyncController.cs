using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.ViewModels;

namespace Playlister.Controllers;

[ValidateTokenCookie]
public class SyncController : Controller
{
    public const string Name = "Sync";

    public Task<IActionResult> Index()
    {
        return Task.FromResult<IActionResult>( View( new SyncViewModel() ) );
    }
}
