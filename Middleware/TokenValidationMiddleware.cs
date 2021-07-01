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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger,
            IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _next = next;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        public async Task Invoke(HttpContext context, IAccessTokenRepository tokenRepository)
        {
            _logger.LogTrace($"Entered {nameof(TokenValidationMiddleware)}");
            Endpoint? endpoint = context.Features.Get<IEndpointFeature>().Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ValidateTokenAttribute>();

            bool valid = false;

            if (attribute is null)
            {
                _logger.LogTrace($"There is no ValidateTokenAttribute on the endpoint {endpoint?.DisplayName}");
                valid = true;
            }
            // valid if there is an AccessToken header with a valid token
            else if (AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"],
                out AuthenticationHeaderValue? authHeader))
            {
                _logger.LogTrace($"Validating access to endpoint {endpoint?.DisplayName}");
                /*
                 * TODO: instead of "Bearer xyz", the Authorization Header is just "xyz", so instead of Parameter,
                 * it's being set as Scheme. This seems wrong, so need to investigate what's going on.
                 */
                // string authToken = authHeader.Parameter!;
                string authToken = authHeader.Scheme;
                _logger.LogTrace($"auth token = {authToken}");

                if (tokenRepository.Get(authToken) is { } token)
                {
                    if (token.Expiration > DateTime.Now)
                    {
                        valid = true;
                        _httpContextAccessor.HttpContext!.Items["AccessToken"] = authToken;
                        _logger.LogTrace("Token is valid");
                    }
                    else
                    {
                        _logger.LogWarning($"Cache entry expiration {token.Expiration} has passed.");
                        tokenRepository.RemoveAccessToken(authToken);

                        try
                        {
                            UserAccessToken refreshedToken =
                                await _mediator.Send(new TokenRefreshCommand(token.RefreshToken));

                            _logger.LogDebug(
                                $"Refreshed token:{Environment.NewLine}{JsonUtility.PrettyPrint(refreshedToken)}");

                            // update Authorization header
                            context.Request.Headers["Authorization"] = $"Bearer {refreshedToken.AccessToken}";
                            valid = true;
                        }
                        catch (ApiException e)
                        {
                            _logger.LogWarning($"Token Refresh failed: {e.Message}.");
                        }
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
