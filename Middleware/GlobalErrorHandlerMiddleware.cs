using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Playlister.Middleware
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                HttpResponse? response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case Refit.ApiException e:
                        // custom application error
                        _logger.LogError($"Refit ApiException: `{e.ReasonPhrase} {e.Content}`");
                        response.StatusCode = (int) e.StatusCode;
                        _logger.LogError($"Authorization Header: {e.RequestMessage.Headers.Authorization}");
                        break;
                    default:
                        // unhandled error
                        _logger.LogError($"Exception was thrown:{Environment.NewLine}{error}");
                        break;
                }

                throw;
            }
        }
    }
}
