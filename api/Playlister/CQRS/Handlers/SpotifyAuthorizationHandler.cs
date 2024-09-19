using Playlister.CQRS.Queries;
using Playlister.Models.SpotifyAccounts;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class SpotifyAuthorizationHandler( IAuthService authService ) {
    private readonly IAuthService _authService = authService;

    public Task<Uri> GetAuthorizationUrl( ) {
        return Task.FromResult( _authService.GetSpotifyAuthUrl( ) );
    }

    public async Task<Guid> GetSpotifyAccessToken( GetAccessTokenQuery query , CancellationToken ct = default ) {
        return await _authService.GetAccessToken(
            new AuthorizationResult {
                Code = query.Code ,
                State = query.State
            } ,
            ct
        );
    }

    public async Task<Guid> RefreshToken( RefreshTokenQuery query , CancellationToken ct = default ) {
        return await _authService.RefreshSpotifyToken( query , ct );
    }
}
