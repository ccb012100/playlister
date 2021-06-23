using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Playlister.Utilities;

namespace Playlister.CQRS
{
    /// <summary>
    /// Log Request/Response bodies from Mediatr Handlers.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
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
            _logger.LogTrace(
                $"{typeof(TRequest).Name} => REQUEST:{Environment.NewLine}{JsonUtility.PrettyPrint(request)}");

            TResponse response = await next();

            _logger.LogTrace(
                $"{typeof(TRequest).Name} => RESPONSE:{Environment.NewLine}{JsonUtility.PrettyPrint(response)}");

            return response;
        }
    }
}
