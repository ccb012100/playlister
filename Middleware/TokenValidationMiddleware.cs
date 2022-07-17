using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Repositories;
using Playlister.Utilities;
using Refit;

namespace Playlister.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenValidationMiddleware> _logger;
        private readonly IMediator _mediator;

        public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger,
            IMediator mediator)
        {
            _next = next;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogDebug($"Entered {nameof(TokenValidationMiddleware)}");
            Endpoint? endpoint = context.Features.Get<IEndpointFeature>()!.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ValidateTokenAttribute>();

            if (attribute is null)
            {
                _logger.LogDebug($"There is no ValidateTokenAttribute on the endpoint {endpoint?.DisplayName}");
                await _next(context); // call action in Controller
                return;
            }

            // valid if there is an AccessToken header with a valid token
            if (!AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"],
                out AuthenticationHeaderValue? authHeader))
            {
                _logger.LogWarning($"Was unable to parse an AuthenticationHeaderValue from the Request");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            _logger.LogDebug($"Validating access to endpoint {endpoint?.DisplayName}");

            if (authHeader.Scheme != "Bearer")
            {
                _logger.LogWarning($"Authorization header has invalid Scheme {authHeader.Scheme}");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            string authToken = authHeader.Parameter!;
            _logger.LogDebug($"auth token = {authToken}");

            if (!string.IsNullOrWhiteSpace(authToken))
            {
                _logger.LogDebug("Token is valid");
                await _next(context);
                return;
            }

            _logger.LogWarning("Auth token was not found in the Authorization Header");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
