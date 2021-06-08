using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Playlister.Models;

namespace Playlister
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public TokenValidationMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            var attribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata
                .GetMetadata<ValidateTokenAttribute>();

            bool valid = false;

            if (attribute == null)
            {
                valid = true;
            }
            else
            {
                // valid if there is an AccessToken header with a valid token
                if (AuthenticationHeaderValue.TryParse(context.Request.Headers["AccessToken"],
                    out AuthenticationHeaderValue? authHeader))
                {
                    // strip out text "Bearer " from auth header value
                    string authToken = authHeader.Parameter!.Substring("Bearer ".Length);

                    if (_cache.TryGetValue(authHeader.Parameter, out UserAccessToken cacheEntry) &&
                        cacheEntry.Expiration > DateTime.Now)
                    {
                        valid = true;
                    }
                }
            }

            if (valid)
            {
                await _next(context); // call action in Controller
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
    }
}
