using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class SpotifyAuthUrlHandler : IRequestHandler<GetAuthUrlCommand, Uri>
{
    private readonly IAuthService _authService;

    public SpotifyAuthUrlHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Uri> Handle(GetAuthUrlCommand command, CancellationToken ct)
    {
        return Task.FromResult(_authService.GetSpotifyAuthUrl());
    }
}
