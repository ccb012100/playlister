using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Extensions;
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
        private readonly IHostApplicationLifetime _appLifetime;

        public CurrentUserController(
            IMediator mediator,
            IAccessTokenUtility tokenUtility,
            ILogger<CurrentUserController> logger,
            IHostApplicationLifetime appLifetime
        )
            : base(mediator, tokenUtility)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }

        /// <summary>
        ///     Get the User who was assigned the Access Token in the Authorization Header.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PrivateUserObject> Get()
        {
            PrivateUserObject user = await _mediator.Send(new GetCurrentUserCommand(AccessToken));

            return user;
        }

        /// <summary>
        ///     Get the current user's Playlists.
        /// </summary>
        /// <returns></returns>
        [HttpGet("playlists")]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            Stopwatch sw = new();
            sw.Start();

            IEnumerable<Playlist> lists = await _mediator.Send(
                new GetCurrentUserPlaylistsCommand(AccessToken)
            );

            sw.Stop();
            _logger.LogInformation(
                "Retrieved current user's {PlaylistCount} playlists. Total time: {Elapsed}",
                lists.Count(),
                sw.Elapsed.ToLogString()
            );

            return Ok(lists);
        }

        /// <summary>
        ///     Update list of current user's playlists.
        /// </summary>
        /// <returns></returns>
        [HttpPost("playlists")]
        public async Task<ActionResult> UpdateCurrentUserPlaylists()
        {
            Stopwatch sw = new();
            sw.Start();

            int total = await _mediator.Send(new UpdateCurrentUserPlaylistsCommand(AccessToken));
            sw.Stop();

            _logger.LogInformation(
                "Updated current user's {Total} playlists. Total time: {Elapsed}",
                total,
                sw.Elapsed.ToLogString()
            );
            return NoContent();
        }

        /// <summary>
        ///     Update list of current user's playlists.
        /// </summary>
        /// <returns></returns>
        // TODO: move this to a more appropriate controller
        [HttpPost("stop-application")]
        public ActionResult StopApplication()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                _appLifetime.StopApplication();
            });
            return NoContent();
        }
    }
}
