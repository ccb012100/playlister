using Microsoft.AspNetCore.Mvc;
using Playlister.ViewModels;

namespace Playlister.Controllers;

public class SyncController : Controller
{
    public Task<IActionResult> Index()
    {
        return Task.FromResult<IActionResult>(View(new SyncViewModel()));
    }
}
