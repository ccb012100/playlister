using Playlister.Models;

namespace Playlister.Services;

public static class TokenService
{
    public const string UserTokenCookieName = "user-token";

    private static AuthToken s_authToken = new()
    {
        ViewToken = Guid.Empty,
        UserAccessToken = new UserAccessToken
        {
            AccessToken = "",
            RefreshToken = null,
            Expiration = DateTime.MinValue
        }
    };

    public static Guid AddToken(UserAccessToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token.AccessToken);

        Guid viewToken = Guid.NewGuid();

        s_authToken = new AuthToken
        {
            ViewToken = viewToken,
            UserAccessToken = token
        };

        return viewToken;
    }

    public static UserAccessToken GetToken(Guid viewToken)
    {
        if (viewToken == Guid.Empty)
        {
            throw new ArgumentException("Guid cannot be Guid.Empty", nameof(viewToken));
        }

        if (viewToken != s_authToken.ViewToken)
        {
            throw new InvalidOperationException($"Invalid user token: {viewToken}");
        }

        return s_authToken.UserAccessToken;
    }

    private class AuthToken
    {
        public required Guid ViewToken { get; init; }
        public required UserAccessToken UserAccessToken { get; init; }
    }
}
