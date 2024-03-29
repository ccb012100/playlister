using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Utilities;

namespace Playlister.Controllers
{
    [ValidateToken, ApiController, Route("api/playlists/{playlistId}")]
    public class PlaylistController : BaseController
    {
        public PlaylistController(IMediator mediator, IAccessTokenUtility tokenUtility)
            : base(mediator, tokenUtility) { }

        [HttpPost("tracks")]
        public async Task<ActionResult> UpdateTracks(string playlistId)
        {
            await _mediator.Send(new UpdatePlaylistCommand(AccessToken, playlistId));

            return NoContent();
        }

        [HttpPost("sync")]
        public async Task<ActionResult> SyncPlaylist(string playlistId)
        {
            await _mediator.Send(new SyncPlaylistCommand(AccessToken, playlistId));

            return NoContent();
        }
    }
}
