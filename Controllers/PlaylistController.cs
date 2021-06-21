using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Requests;
using Playlister.Models;

namespace Playlister.Controllers
{
    [ValidateToken, ApiController, Route("api/playlists")]
    public class PlaylistController : BaseController
    {
        public PlaylistController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{playlistId}")]
        public async Task<Playlist> Get(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("tracks")]
        public async Task<ActionResult> UpdateTracks([FromBody] MinimalPlaylist minimalPlaylistIds)
        {
            await Mediator.Send(new UpdatePlaylistItemsRequest(minimalPlaylistIds));

            return NoContent();
        }
    }
}
