using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Playlister.Api.Services;

namespace Playlister.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StartController : Controller
    {
        private readonly ISpotifyAccountsService _spotifyAccountsService;

        public StartController(ISpotifyAccountsService spotifyAccountsService)
        {
            _spotifyAccountsService = spotifyAccountsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            object? result = await _spotifyAccountsService.Authorize();

            return Ok(result);
        }
    }
}
