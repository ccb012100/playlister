using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.Models.SpotifyApi;
using Playlister.Requests;

namespace Playlister.Controllers
{
    [ValidateToken, ApiController, Route("api/user")]
    public class CurrentSpotifyUserController : BaseController
    {
        public CurrentSpotifyUserController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Get the User who was assigned the Access Token in the Authorization Header.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PrivateUserObject> Get()
        {
            PrivateUserObject user = await Mediator.Send(new CurrentUserRequest());
            return user;
        }

        /// <summary>
        /// Get the current user's Playlists.
        /// </summary>
        /// <param name="offset">The index of the first playlist to return. Default: 0 (the first object). Maximum offset: 100.000. Use with limit to get the next set of playlists.</param>
        /// <param name="limit">The maximum number of playlists to return. Default: 20. Minimum: 1. Maximum: 50.</param>
        /// <returns></returns>
        [HttpGet("playlists")]
        public async Task<ActionResult<PagingObject<SimplifiedPlaylistObject>>> GetPlaylists([FromQuery] int? offset,
            [FromQuery] int? limit)
        {
            PagingObject<SimplifiedPlaylistObject> user =
                await Mediator.Send(new CurrentUserPlaylistsRequest(offset, limit));
            return Ok(user);
        }

        [HttpPost("playlists")]
        public async Task<ActionResult> UpdatePlaylist(UpdatePlaylistRequest request)
        {
            await Mediator.Send(request);

            return Ok("Playlists have been updated!");
        }
    }
}
