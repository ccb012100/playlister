using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class SpotifyAuthUrlHandler : ICommandHandler
{
    private readonly IAuthService _authService;

    public SpotifyAuthUrlHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Uri> Handle(GetAuthUrlCommand command, CancellationToken ct = default)
    {
        return Task.FromResult(_authService.GetSpotifyAuthUrl());
    }
}
