using Microsoft.AspNetCore.Mvc;
using Playlister.ViewModels;

namespace Playlister.Controllers;

public class SyncController : Controller
{
    public Task<IActionResult> Index() => Task.FromResult<IActionResult>(View(new SyncViewModel()));
}
