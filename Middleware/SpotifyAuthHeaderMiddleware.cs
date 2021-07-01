using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Playlister.Middleware
{
    /// <summary>
    /// Middleware to add Authorization Header with Access Token in outbound Refit requests.
    /// </summary>
    internal class SpotifyAuthHeaderMiddleware : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<SpotifyAuthHeaderMiddleware> _logger;

        public SpotifyAuthHeaderMiddleware(IHttpContextAccessor contextAccessor,
            ILogger<SpotifyAuthHeaderMiddleware> logger)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            _logger.LogInformation("Entering auth header middleware");

            HttpContext? httpContext = _contextAccessor.HttpContext;

            if (httpContext is not null)
            {
                var accessToken = (string?) httpContext.Items["AccessToken"];

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }
            else
            {
                _logger.LogWarning($"No http context was found for request:\n{request.RequestUri}");
            }

            return await base.SendAsync(request, ct).ConfigureAwait(false);
        }
    }
}
