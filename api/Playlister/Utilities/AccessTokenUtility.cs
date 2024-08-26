using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Playlister.Utilities
{
    public class AccessTokenUtility : IAccessTokenUtility
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AccessTokenUtility> _logger;

        public AccessTokenUtility(
            IHttpContextAccessor contextAccessor,
            ILogger<AccessTokenUtility> logger
        )
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public string GetAccessTokenFromCurrentHttpContext()
        {
            if (_contextAccessor is null)
            {
                throw new InvalidOperationException("IHttpContextAccessor is null");
            }

            if (_contextAccessor.HttpContext is null)
            {
                throw new InvalidOperationException("httpContext");
            }

            if (
                !AuthenticationHeaderValue.TryParse(
                    _contextAccessor.HttpContext.Request.Headers["Authorization"],
                    out AuthenticationHeaderValue? authHeader
                )
            )
            {
                throw new InvalidOperationException(
                    "No Authorization header found on HttpContext.Request"
                );
            }

            string token =
                authHeader.Parameter
                ?? throw new NullReferenceException(
                    "The Authentication Header was present, but the Parameter was null"
                );

            _logger.LogDebug("Found access token {Token} on HttpContext", token);

            return token;
        }
    }
}
