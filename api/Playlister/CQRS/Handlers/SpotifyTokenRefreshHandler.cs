using System.Text;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Queries;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.RefitClients;

namespace Playlister.CQRS.Handlers;

public class SpotifyTokenRefreshHandler
{
    private readonly ISpotifyAccountsApi _api;
    private readonly SpotifyOptions _options;

    public SpotifyTokenRefreshHandler( ISpotifyAccountsApi api, IOptions<SpotifyOptions> options )
    {
        _api = api;
        _options = options.Value;
    }

    public async Task<AuthenticationToken> Handle( RefreshTokenQuery query, CancellationToken ct = default )
    {
        string authParam =
            Convert.ToBase64String( Encoding.UTF8.GetBytes( $"{_options.ClientId}:{_options.ClientSecret}" ) );

        SpotifyAccessToken token = await _api.RefreshTokenAsync(
            authParam,
            new RefreshTokenQuery.BodyParams( query.RefreshToken ),
            ct
        );

        return token.ToUserAccessToken();
    }
}
