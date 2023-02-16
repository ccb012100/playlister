using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Utilities;

namespace Playlister.Controllers
{
    [ValidateToken]
    [ApiController]
    [Route("api/user")]
    public class CurrentUserController : BaseController
    {
        private readonly ILogger<CurrentUserController> _logger;

        public CurrentUserController(IMediator mediator, IAccessTokenUtility tokenUtility,
            ILogger<CurrentUserController> logger)
            : base(mediator, tokenUtility)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Get the User who was assigned the Access Token in the Authorization Header.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PrivateUserObject> Get()
        {
            PrivateUserObject user =
                await Mediator.Send(new GetCurrentUserCommand(AccessToken));

            return user;
        }

        /// <summary>
        ///     Get the current user's Playlists.
        /// </summary>
        /// <returns></returns>
        [HttpGet("playlists")]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            var sw = new Stopwatch();
            sw.Start();

            IEnumerable<Playlist> lists = await Mediator.Send(new GetCurrentUserPlaylistsCommand(AccessToken));

            sw.Stop();
            _logger.LogInformation("Retrieved current user's {PlaylistCount} playlists. Total time: {Elapsed}", lists.Count(), sw.Elapsed);

            return Ok(lists);
        }

        /// <summary>
        ///     Update list of current user's playlists.
        /// </summary>
        /// <returns></returns>
        [HttpPost("playlists")]
        public async Task<ActionResult> UpdateCurrentUserPlaylists()
        {
            var sw = new Stopwatch();
            sw.Start();

            var total = await Mediator.Send(new UpdateCurrentUserPlaylistsCommand(AccessToken));
            sw.Stop();

            _logger.LogInformation("Updated current user's {Total} playlists. Total time: {Elapsed}", total, sw.Elapsed);
            return NoContent();
        }
    }
}
