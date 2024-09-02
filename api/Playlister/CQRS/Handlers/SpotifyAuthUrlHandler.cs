using Playlister.CQRS.Queries;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class SpotifyAuthUrlHandler
{
    private readonly IAuthService _authService;

    public SpotifyAuthUrlHandler( IAuthService authService )
    {
        _authService = authService;
    }

    public Task<Uri> Handle( GetAuthUrlQuery query, CancellationToken ct = default )
    {
        return Task.FromResult( _authService.GetSpotifyAuthUrl() );
    }
}
