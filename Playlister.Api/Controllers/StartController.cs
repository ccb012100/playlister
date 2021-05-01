using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Playlister.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StartController : BaseController
    {
        public StartController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            throw new NotImplementedException();
        }
    }
}
