using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Playlister.Middleware
{
    /// <summary>
    /// Middleware to add Authorization Header with Access Token in outbound Refit requests.
    /// </summary>
    internal class SpotifyAuthHeaderMiddleware : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public SpotifyAuthHeaderMiddleware(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // TODO: refresh token here if it has expired etc.

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer",
                    (string?) _contextAccessor.HttpContext!.Items["AccessToken"]);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
