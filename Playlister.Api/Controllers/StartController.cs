using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Playlister.Services;

namespace Playlister.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StartController : Controller
    {
        private readonly ISpotifyAuthorizationService _spotifyAuthService;

        public StartController(ISpotifyAuthorizationService spotifyAuthService)
        {
            _spotifyAuthService = spotifyAuthService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok(DateTime.Now));
        }
    }
}
