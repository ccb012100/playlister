using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.Utilities;

namespace Playlister.Controllers
{
    [ValidateToken, ApiController, Route("api/user")]
    public class CurrentUserController : BaseController
    {
        public CurrentUserController(IMediator mediator, IAccessTokenUtility tokenUtility)
            : base(mediator, tokenUtility)
        {
        }

        /// <summary>
        /// Get the User who was assigned the Access Token in the Authorization Header.
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
        /// Get the current user's Playlists.
        /// </summary>
        /// <returns></returns>
        [HttpGet("playlists")]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            IEnumerable<Playlist> lists = await Mediator.Send(new GetCurrentUserPlaylistsCommand(AccessToken));

            return Ok(lists);
        }

        /// <summary>
        /// Update list of current user's playlists.
        /// </summary>
        /// <returns></returns>
        [HttpPost("playlists")]
        public async Task<ActionResult> UpdateCurrentUserPlaylists()
        {
            await Mediator.Send(new UpdateCurrentUserPlaylistsCommand(AccessToken));

            return NoContent();
        }
    }
}
