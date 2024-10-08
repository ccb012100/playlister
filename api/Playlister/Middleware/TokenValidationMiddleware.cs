using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;

using Playlister.Attributes;
using Playlister.Services.Implementations;

namespace Playlister.Middleware;

/// <summary>
///     Validates endpoints marked with the <see cref="ValidateTokenCookieAttribute" /> attribute. If the <see cref="HttpRequest" /> does not contain a
///     <c>"user-token"</c> cookie, or the value it contains is invalid, the <see cref="HttpResponse" /> is redirected back to <c>/Home</c> to go through
///     the
///     authentication flow
/// </summary>
public class TokenValidationMiddleware(
    RequestDelegate next ,
    ILogger<TokenValidationMiddleware> logger
) {
    private readonly ILogger<TokenValidationMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task Invoke( HttpContext context ) {
        _logger.LogTrace( $"Entered {nameof( TokenValidationMiddleware )}" );
        Endpoint? endpoint = context.Features.Get<IEndpointFeature>( )!.Endpoint;
        ValidateTokenCookieAttribute? attribute = endpoint?.Metadata.GetMetadata<ValidateTokenCookieAttribute>( );

        IRequestCookieCollection cookies = context.Request.Cookies;

        foreach ( (string key, string value) in cookies ) {
            _logger.LogDebug( "Request Cookie => {CookieName} {CookieValue}" , key , value );
        }

        switch (attribute, endpoint) {
            case (null, null ): {
                    _logger.LogDebug(
                        "There is no {Attribute} for {Path}; skipping validation" ,
                        nameof( ValidateTokenCookieAttribute ) ,
                        context.Request.GetDisplayUrl( )
                    );

                    break;
                }
            case (null, not null ): {
                    _logger.LogDebug(
                        "There is no {Attribute} on the endpoint {Endpoint}; skipping validation" ,
                        nameof( ValidateTokenCookieAttribute ) ,
                        endpoint.DisplayName
                    );

                    break;
                }
            case (not null, _ ): {
                    string? cookie = context.Request.Cookies[TokenService.UserTokenCookieName];

                    if ( !TokenService.TryValidateCookie( cookie , out string? errorMessage ) ) {
                        _logger.LogDebug(
                            "Token in \"{Cookie}\" cookie failed validation: {Error}" ,
                            nameof( TokenService.UserTokenCookieName ) ,
                            errorMessage
                        );

                        context.Response.Redirect( "Home/Index" );

                        return;
                    }

                    _logger.LogDebug( "Token is valid" );

                    break;
                }
        }

        await _next( context );
    }
}
