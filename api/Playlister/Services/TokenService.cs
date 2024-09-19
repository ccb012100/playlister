using static System.String;

namespace Playlister.Services;

public static class TokenService {
    public const string UserTokenCookieName = "user-token";

    private static AuthToken s_authToken = new( ) {
        ViewToken = Guid.Empty ,
        AuthenticationToken = new AuthenticationToken {
            AccessToken = "" ,
            RefreshToken = null ,
            ExpirationUtc = DateTime.MinValue
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
    public static Guid SetToken( AuthenticationToken token ) {
        ArgumentException.ThrowIfNullOrWhiteSpace( token.AccessToken );

        Guid viewToken = Guid.NewGuid( );

        s_authToken = new AuthToken {
            ViewToken = viewToken ,
            AuthenticationToken = token
        };

        return viewToken;
    }

    public static Guid RefreshToken( AuthenticationToken token ) {
        ArgumentException.ThrowIfNullOrWhiteSpace( token.AccessToken );

        s_authToken = new AuthToken {
            ViewToken = s_authToken.ViewToken ,
            AuthenticationToken = token
        };

        return s_authToken.ViewToken;
    }

    /// <summary>
    ///     Get the Spotify Authentication Token
    /// </summary>
    /// <param name="viewToken">The key for accessing the Authentication Token</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><paramref name="viewToken" /> is <see cref="Guid.Empty" /></exception>
    /// <exception cref="ArgumentException"><paramref name="viewToken" /> is invalid</exception>
    /// <exception cref="InvalidOperationException"><paramref name="viewToken" /> is an expired</exception>
    public static AuthenticationToken GetAuthenticationToken( Guid viewToken ) {
        ValidateViewToken( viewToken );

        return s_authToken.AuthenticationToken;
    }

    /// <summary>
    ///     Check the validity of a <c>"user-token"</c> cookie
    /// </summary>
    /// <param name="cookie">The cookie to validate</param>
    /// <param name="error">Validation failure reason; <c>null</c> if <paramref name="cookie" /> is valid</param>
    /// <returns><c>true</c> if <paramref name="cookie" /> is valid; else, <c>false</c></returns>
    public static bool TryValidateCookie( string? cookie , out string? error ) {
        error = null;

        if ( IsNullOrWhiteSpace( cookie ) || !Guid.TryParse( cookie , out Guid token ) ) {
            error = $"Token value '{cookie}' found in cookie is not a valid Guid format!";

            return false;
        }

        if ( token != s_authToken.ViewToken ) {
            error = $"Invalid token value '{token}' found in cookie";

            return false;
        }

        if ( s_authToken.AuthenticationToken.ExpirationUtc <= DateTime.UtcNow ) {
            error = $"Token expired at {s_authToken.AuthenticationToken.ExpirationUtc}";
        }

        return true;
    }

    /// <summary>
    /// Get the expiration time for the supplied token.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="viewToken" /> is <see cref="Guid.Empty" /></exception>
    /// <exception cref="ArgumentException"><paramref name="viewToken" /> is invalid</exception>
    /// <exception cref="InvalidOperationException"><paramref name="viewToken" /> is an expired</exception>
    public static DateTime GetTokenExpirationUtc( Guid viewToken ) {
        ValidateViewToken( viewToken );

        return s_authToken.AuthenticationToken.ExpirationUtc;
    }

    /// <summary>
    ///     Throw an <see cref="Exception"/> if <paramref name="viewToken"/> is not a valid token.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="viewToken" /> is <see cref="Guid.Empty" /></exception>
    /// <exception cref="ArgumentException"><paramref name="viewToken" /> is invalid</exception>
    /// <exception cref="InvalidOperationException"><paramref name="viewToken" /> is an expired</exception>
    private static void ValidateViewToken( Guid viewToken ) {
        if ( viewToken == Guid.Empty ) {
            throw new ArgumentException( "Guid cannot be Guid.Empty" , nameof( viewToken ) );
        }

        if ( viewToken != s_authToken.ViewToken ) {
            throw new ArgumentException( $"Invalid user token: {viewToken}" );
        }

        if ( s_authToken.AuthenticationToken.ExpirationUtc <= DateTime.Now ) {
            throw new InvalidOperationException( $"Token expired at {s_authToken.AuthenticationToken.ExpirationUtc}" );
        }
    }

    private class AuthToken {
        /// <summary>
        ///     Used in the <c>"user-token"</c> cookie to avoid exposing the real <b>Spotify</b> token outside the application
        /// </summary>
        public required Guid ViewToken { get; init; }

        /// <summary>
        ///     Represents the actual <b>Spotify</b> Access Token
        /// </summary>
        public required AuthenticationToken AuthenticationToken { get; init; }
    }
}
