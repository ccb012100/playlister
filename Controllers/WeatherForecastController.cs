using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Commands;
using Playlister.Models;

namespace Playlister.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherForecastController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<WeatherForecast> Get() =>
            await _mediator.Send(new NewWeatherForecastCommand {Date = DateTime.Now});
    }
}
