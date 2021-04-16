using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Playlister.Commands;
using Playlister.Models;

namespace Playlister.RequestHandlers
{
    // ReSharper disable once UnusedType.Global
    public class NewWeatherForecastHandler : IRequestHandler<NewWeatherForecastCommand, WeatherForecast>
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        Task<WeatherForecast> IRequestHandler<NewWeatherForecastCommand, WeatherForecast>.Handle(
            NewWeatherForecastCommand command, CancellationToken cancellationToken)
        {
            var rng = new Random();

            return Task.FromResult(
                new WeatherForecast
                {
                    Date = command.Date,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                });
        }
    }
}
