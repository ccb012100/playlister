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
            var accessToken = (string?) _contextAccessor.HttpContext!.Items["AccessToken"];

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
