using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.Models;

namespace Playlister.Controllers
{
    [ValidateToken, ApiController, Route("api/playlists")]
    public class PlaylistController : BaseController
    {
        public PlaylistController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        public async Task<Playlist> Get(string id)
        {
            throw new NotImplementedException();
        }
    }
}