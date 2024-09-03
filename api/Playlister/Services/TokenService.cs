using Playlister.Models;
using static System.String;

namespace Playlister.Services;

public static class TokenService
{
    public const string UserTokenCookieName = "user-token";

    private static AuthToken s_authToken = new()
    {
        ViewToken = Guid.Empty,
        AuthenticationToken = new AuthenticationToken
        {
            AccessToken = "",
            RefreshToken = null,
            Expiration = DateTime.MinValue
        }
    };

    /// <summary>
    ///     Set the current <see cref="AuthenticationToken" /> in the application
    /// </summary>
    /// <param name="token"></param>
    /// <returns>
    ///     The ViewToken, which is set in the <c>"user-token"</c> cookie and is used a key for retrieving the current
    ///     <see cref="AuthenticationToken" />
    /// </returns>
    public static Guid SetToken( AuthenticationToken token )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( token.AccessToken );

        Guid viewToken = Guid.NewGuid();

        s_authToken = new AuthToken
        {
            ViewToken = viewToken,
            AuthenticationToken = token
        };

        return viewToken;
    }

    public static AuthenticationToken GetToken( Guid viewToken )
    {
        if (viewToken == Guid.Empty)
        {
            throw new ArgumentException( "Guid cannot be Guid.Empty", nameof(viewToken) );
        }

        return viewToken == s_authToken.ViewToken
            ? s_authToken.AuthenticationToken
            : throw new InvalidOperationException( $"Invalid user token: {viewToken}" );
    }

    public static bool TryValidateCookie( string? cookie, out string? error )
    {
        if (IsNullOrWhiteSpace( cookie ) || !Guid.TryParse( cookie, out Guid token ))
        {
            error = $"Token value '{cookie}' found in cookie is not a valid Guid format!";

            return false;
        }

        if (!IsValidToken( token ))
        {
            error = $"Invalid token value '{token}' found in cookie";

            return false;
        }

        error = null;

        return true;
    }

    private static bool IsValidToken( Guid viewToken )
    {
        return s_authToken.ViewToken != Guid.Empty && viewToken == s_authToken.ViewToken;
    }

    private class AuthToken
    {
        public required Guid ViewToken { get; init; }
        public required AuthenticationToken AuthenticationToken { get; init; }
    }
}
