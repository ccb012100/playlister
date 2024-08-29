using Refit;

namespace Playlister.Middleware;

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
        catch (Exception ex)
        {
            HttpResponse response = context.Response;
            response.ContentType = "application/json";

            switch (ex)
            {
                case ApiException e:
                    // custom application error
                    _logger.LogError("Unhandled Refit ApiException: `{ReasonPhrase} {Content}`", e.ReasonPhrase, e.Content);
                    response.StatusCode = (int)e.StatusCode;
                    _logger.LogError("Unhandled Authorization Header: {Headers}", e.RequestMessage.Headers.Authorization);

                    break;
                default:
                    _logger.LogError(ex, "Unhandled {ExceptionType}", ex.GetType());

                    break;
            }

            throw;
        }
    }
}
