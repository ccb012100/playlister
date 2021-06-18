using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Playlister.Attributes;
using Playlister.Models;

namespace Playlister.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TokenValidationMiddleware> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenValidationMiddleware(RequestDelegate next, IMemoryCache cache,
            ILogger<TokenValidationMiddleware> logger, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogDebug($"Entered {nameof(TokenValidationMiddleware)}");
            Endpoint? endpoint = context.Features.Get<IEndpointFeature>().Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ValidateTokenAttribute>();

            bool valid = false;

            if (attribute == null)
            {
                _logger.LogDebug($"There is no ValidateTokenAttribute on the endpoint {endpoint?.DisplayName}");
                valid = true;
            }
            // valid if there is an AccessToken header with a valid token
            else if (AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"],
                out AuthenticationHeaderValue? authHeader))
            {
                _logger.LogDebug($"Validating access to endpoint {endpoint?.DisplayName}");
                /*
                 * TODO: instead of "Bearer xyz", the Authorization Header is just "xyz", so instead of Parameter,
                 * it's being set as Scheme. This seems wrong, so need to investigate what's going on.
                 */
                // string authToken = authHeader.Parameter!;
                string authToken = authHeader.Scheme;
                _logger.LogDebug($"auth token = {authToken}");

                if (_cache.TryGetValue(authToken, out UserAccessInfo cacheEntry))
                {
                    if (cacheEntry.Expiration > DateTime.Now)
                    {
                        valid = true;
                        _httpContextAccessor.HttpContext!.Items["AccessToken"] = authToken;
                        _logger.LogDebug("Token is valid");
                    }
                    else
                    {
                        _logger.LogWarning($"Cache entry expiration {cacheEntry.Expiration} has passed.");
                        // TODO: use refresh token to get new access token
                    }
                }
                else
                {
                    _logger.LogWarning("Auth token was not found in the cache");
                }
            }
            else
            {
                _logger.LogWarning(
                    $"Was unable to parse an AuthenticationHeaderValue from `context.Request.Headers[\"Authorization\"]`");
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
