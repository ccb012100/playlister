using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Refit;

namespace Playlister.Middleware
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

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
                HttpResponse response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case ApiException e:
                        // custom application error
                        _logger.LogError("Refit ApiException: `{ReasonPhrase} {Content}`", e.ReasonPhrase, e.Content);
                        response.StatusCode = (int)e.StatusCode;
                        _logger.LogError("Authorization Header: {Headers}", e.RequestMessage.Headers.Authorization);

                        break;
                }

                throw;
            }
        }
    }
}
