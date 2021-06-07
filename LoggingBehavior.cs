using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.Extensions;

namespace Playlister
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogDebug($"{typeof(TRequest).Name} => REQUEST:{Environment.NewLine}{request.ToPrettyPrintJson()}");
            TResponse response = await next();
            _logger.LogDebug(
                $"{typeof(TRequest).Name} => RESPONSE:{Environment.NewLine}{response.ToPrettyPrintJson()}");

            return response;
        }
    }
}
