using Playlister.Services;

namespace Playlister.Utilities;

public class AccessTokenUtility : IAccessTokenUtility
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccessTokenUtility( IHttpContextAccessor httpContextAccessor )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     To avoid unhandled <see cref="InvalidOperationException" />s, this should only be used by methods marked with the
    ///     <see cref="Playlister.Attributes .ValidateTokenCookieAttribute" />
    /// </summary>
    /// <returns>The user token</returns>
    /// <exception cref="InvalidOperationException">
    ///     The method is called outside an <see cref="HttpContext" />
    ///     or the <c>"user-token"</c> cookie is missing/has an invalid value
    /// </exception>
    public string GetTokenFromUserCookie()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new InvalidOperationException( "HttpContext is null" );
        }

        string? cookie = _httpContextAccessor.HttpContext.Request.Cookies[TokenService.UserTokenCookieName];

        if (string.IsNullOrWhiteSpace( cookie ))
        {
            throw new InvalidOperationException( $"{TokenService.UserTokenCookieName} cookie missing!" );
        }

        if (!Guid.TryParse( cookie, out Guid viewToken ))
        {
            throw new InvalidOperationException( $"Invalid {TokenService.UserTokenCookieName} cooke value: {cookie}" );
        }

        return TokenService.GetToken( viewToken ).AccessToken;
    }
}
