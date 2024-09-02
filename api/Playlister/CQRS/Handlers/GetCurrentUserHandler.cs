using Playlister.CQRS.Commands;
using Playlister.Models.SpotifyApi;
using Playlister.RefitClients;

namespace Playlister.CQRS.Handlers;

public class GetCurrentUserHandler : ICommandHandler
{
    private readonly ISpotifyApi _api;

    public GetCurrentUserHandler( ISpotifyApi api )
    {
        _api = api;
    }

    public async Task<PrivateUserObject> Handle( GetCurrentUserCommand command, CancellationToken ct = default )
    {
        return await _api.GetCurrentUserAsync( command.AccessToken, ct );
    }
}
