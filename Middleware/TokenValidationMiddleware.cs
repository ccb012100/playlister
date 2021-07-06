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

        public async Task Invoke(HttpContext context, IAccessTokenRepository tokenRepository)
        {
            _logger.LogTrace($"Entered {nameof(TokenValidationMiddleware)}");
            Endpoint? endpoint = context.Features.Get<IEndpointFeature>().Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ValidateTokenAttribute>();

            if (attribute is null)
            {
                _logger.LogTrace($"There is no ValidateTokenAttribute on the endpoint {endpoint?.DisplayName}");
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

            _logger.LogTrace($"Validating access to endpoint {endpoint?.DisplayName}");

            if (authHeader.Scheme != "Bearer")
            {
                _logger.LogWarning($"Authorization header has invalid Scheme {authHeader.Scheme}");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            string authToken = authHeader.Parameter!;
            _logger.LogTrace($"auth token = {authToken}");

            if (tokenRepository.Get(authToken) is { } token)
            {
                if (token.Expiration > DateTime.Now)
                {
                    _logger.LogTrace("Token is valid");
                    await _next(context);
                    return;
                }

                _logger.LogWarning($"Cache entry expiration {token.Expiration} has passed.");
                tokenRepository.RemoveAccessToken(authToken);

                if (token.RefreshToken is not null)
                {
                    try
                    {
                        UserAccessToken refreshedToken =
                            await _mediator.Send(new RefreshTokenCommand(token.RefreshToken));

                        _logger.LogDebug($"Refreshed token:{JsonUtility.PrettyPrint(refreshedToken)}");

                        // update Authorization header
                        context.Request.Headers["Authorization"] = $"Bearer {refreshedToken.AccessToken}";
                        await _next(context);
                        return;
                    }
                    catch (ApiException e)
                    {
                        _logger.LogWarning($"Token Refresh failed: {e.Message}.");
                    }
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            _logger.LogWarning("Auth token was not found in the cache");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
