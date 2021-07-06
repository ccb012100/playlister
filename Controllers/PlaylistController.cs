using System.Diagnostics;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Utilities;

namespace Playlister.Controllers
{
    [ValidateToken, ApiController, Route("api/playlists")]
    public class PlaylistController : BaseController
    {
        public PlaylistController(IMediator mediator, IAccessTokenUtility tokenUtility) : base(mediator, tokenUtility)
        {
        }

        [HttpPost("tracks/{playlistId}")]
        public async Task<ActionResult> UpdateTracks(string playlistId)
        {
            await Mediator.Send(new UpdatePlaylistCommand(AccessToken, playlistId));

            return NoContent();
        }
    }
}
