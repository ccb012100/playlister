using Playlister.Models.SpotifyAccounts;
using Playlister.RefitClients;

namespace Playlister.Tests.Integration.Mocks;

/// <summary>
///     Mock implementation of ISpotifyAccountsApi for testing.
/// </summary>
public class MockSpotifyAccountsApi : ISpotifyAccountsApi {
    public Task<SpotifyAccessToken> RequestAccessTokenAsync(
        string authHeaderParam ,
        AccessTokenRequestParams bodyRequestParams ,
        CancellationToken ct
    ) {
        var token = new SpotifyAccessToken {
            AccessToken = "mock_access_token" ,
            TokenType = "Bearer" ,
            ExpiresIn = 3600 ,
            RefreshToken = "mock_refresh_token" ,
            Scope = "playlist-read-private playlist-read-collaborative"
        };

        return Task.FromResult( token );
    }

    public Task<SpotifyAccessToken> RefreshTokenAsync(
        string authHeaderParam ,
        TokenRefreshParams bodyParams ,
        CancellationToken ct
    ) {
        var token = new SpotifyAccessToken {
            AccessToken = "mock_refreshed_access_token" ,
            TokenType = "Bearer" ,
            ExpiresIn = 3600 ,
            RefreshToken = "mock_refresh_token" ,
            Scope = "playlist-read-private playlist-read-collaborative"
        };

        return Task.FromResult( token );
    }
}
