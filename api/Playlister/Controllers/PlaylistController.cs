using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ValidateAuthHeaderToken]
[ApiController]
[Route("api/playlists/{playlistId}")]
public class PlaylistController : BaseController
{
    public PlaylistController(IMediator mediator, IAccessTokenUtility tokenUtility)
        : base(mediator, tokenUtility) { }

    [HttpPost("tracks")]
    public async Task<ActionResult> UpdateTracks(string playlistId)
    {
        await Mediator.Send(new UpdatePlaylistCommand(AuthHeaderAccessToken, playlistId));

        return NoContent();
    }

    [HttpPost("sync")]
    public async Task<ActionResult> SyncPlaylist(string playlistId)
    {
        await Mediator.Send(new SyncPlaylistCommand(AuthHeaderAccessToken, playlistId));

        return NoContent();
    }
}
