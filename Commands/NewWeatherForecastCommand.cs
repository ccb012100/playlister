using System;
using MediatR;
using Playlister.Models;

namespace Playlister.Commands
{
    public record NewWeatherForecastCommand : IRequest<WeatherForecast>
    {
        public DateTime Date { get; init; }
    }
}