using System.Net.Http.Headers;
using Playlister.Services;

namespace Playlister.Utilities;

public class AccessTokenUtility : IAccessTokenUtility
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AccessTokenUtility> _logger;

    public AccessTokenUtility(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AccessTokenUtility> logger
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string GetAccessTokenFromRequestAuthHeader()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new InvalidOperationException("httpContext");
        }

        if (!AuthenticationHeaderValue.TryParse(
                _httpContextAccessor.HttpContext.Request.Headers.Authorization,
                out AuthenticationHeaderValue? authHeader))
        {
            throw new InvalidOperationException("No Authorization header found on HttpContext.Request");
        }

        string token = authHeader.Parameter
                       ?? throw new NullReferenceException("The Authentication Header was present, but the Parameter was null");

        _logger.LogDebug("Found access token {Token} on HttpContext", token);

        return token;
    }

    public string GetTokenFromUserCookie()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new InvalidOperationException("HttpContext is null");
        }

        string? cookie = _httpContextAccessor.HttpContext.Request.Cookies[TokenService.UserTokenCookieName];

        if (string.IsNullOrWhiteSpace(cookie))
        {
            throw new InvalidOperationException($"{TokenService.UserTokenCookieName} cookie missing!");
        }

        if (!Guid.TryParse(cookie, out Guid viewToken))
        {
            throw new InvalidOperationException($"Invalid {TokenService.UserTokenCookieName} cooke value: {cookie}");
        }

        return TokenService.GetToken(viewToken).AccessToken;
    }
}
