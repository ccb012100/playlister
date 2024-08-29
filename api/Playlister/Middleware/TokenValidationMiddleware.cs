using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Playlister.Attributes;

namespace Playlister.Middleware;

/// <summary>
/// Validate that the HTTP Request has a valid Authentication Token attached
/// </summary>
public class TokenValidationMiddleware
{
    private readonly ILogger<TokenValidationMiddleware> _logger;
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(
        RequestDelegate next,
        ILogger<TokenValidationMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogDebug($"Entered {nameof(TokenValidationMiddleware)}");

        Endpoint? endpoint = context.Features.Get<IEndpointFeature>()!.Endpoint;
        ValidateAuthHeaderTokenAttribute? attribute = endpoint?.Metadata.GetMetadata<ValidateAuthHeaderTokenAttribute>();

        if (attribute is null)
        {
            if (endpoint is null)
            {
                _logger.LogDebug("There is no ValidateAuthHeaderTokenAttribute for {Path}", context.Request.GetDisplayUrl());
            }
            else
            {
                _logger.LogDebug("There is no ValidateAuthHeaderTokenAttribute on the endpoint {Endpoint}", endpoint.DisplayName);
            }

            await _next(context); // call action in Controller

            return;
        }

        if (!AuthenticationHeaderValue.TryParse(context.Request.Headers.Authorization, out AuthenticationHeaderValue? authHeader))
        {
            _logger.LogWarning("Was unable to parse an AuthenticationHeaderValue from the Request");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await _next(context);

            return;
        }

        if (endpoint is null)
        {
            _logger.LogDebug("Validating access to {Path}", context.Request.GetDisplayUrl());
        }
        else
        {
            _logger.LogDebug("Validating access to endpoint {Endpoint}", endpoint.DisplayName ?? "<null>");
        }

        if (authHeader.Scheme != "Bearer")
        {
            _logger.LogWarning("Authorization header has invalid Scheme {AuthScheme}", authHeader.Scheme);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return;
        }

        string authToken = authHeader.Parameter!;

        _logger.LogDebug("auth token = {AuthToken}", authToken);

        if (string.IsNullOrWhiteSpace(authToken))
        {
            _logger.LogWarning("Auth token was not found in the Authorization Header");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return;
        }

        _logger.LogDebug("Token is valid");

        await _next(context);
    }
}
