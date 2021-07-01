using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Requests;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.Controllers
{
    [ValidateToken, ApiController, Route("api/user")]
    public class CurrentSpotifyUserController : BaseController
    {
        public CurrentSpotifyUserController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// GetAll the User who was assigned the Access Token in the Authorization Header.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PrivateUserObject> Get()
        {
            PrivateUserObject user = await Mediator.Send(new CurrentUserCommand());

            return user;
        }

        /// <summary>
        /// GetAll the current user's Playlists.
        /// </summary>
        /// <returns></returns>
        [HttpGet("playlists")]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            IEnumerable<Playlist> lists = await Mediator.Send(new CurrentUserPlaylistsCommand());

            return Ok(lists);
        }

        /// <summary>
        /// Update list of current user's playlists.
        /// </summary>
        /// <returns></returns>
        [HttpPost("playlists")]
        public async Task<ActionResult> UpdateCurrentUserPlaylists()
        {
            await Mediator.Send(new CurrentUserUpdatePlaylistsCommand());

            return NoContent();
        }
    }
}
