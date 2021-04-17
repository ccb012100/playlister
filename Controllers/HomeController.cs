using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Playlister.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok(DateTime.Now));
        }
    }
}
