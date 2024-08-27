using MediatR;
using Playlister.Utilities;

namespace Playlister.CQRS
{
    /// <summary>
    ///     Log Request/Response bodies from Mediatr Handlers.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogTrace(
                "{RequestType} => Mediator Request: {Request}", typeof(TRequest).Name, JsonUtility.PrettyPrint(request));

            TResponse response = await next();

            _logger.LogTrace(
                "{ResponseType} => Mediator Response: {Response}", typeof(TResponse).Name, JsonUtility.PrettyPrint(response));

            return response;
        }
    }
}
