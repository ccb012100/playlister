using Playlister.CQRS.Queries;
using Playlister.Models.SpotifyApi;
using Playlister.RefitClients;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class CurrentUserHandler
{
    private readonly ISpotifyApi _api;
    private readonly IPlaylistService _playlistService;

    public CurrentUserHandler( ISpotifyApi api, IPlaylistService playlistService )
    {
        _api = api;
        _playlistService = playlistService;
    }

    public async Task<PrivateUserObject> Get( GetCurrentUserQuery query, CancellationToken ct = default )
    {
        return await _api.GetCurrentUserAsync( query.AccessToken, ct );
    }

    public async Task<IEnumerable<Playlist>> GetPlaylists(
        GetCurrentUserPlaylistsQuery query,
        CancellationToken ct = default
    )
    {
        return await _playlistService.GetUserPlaylistsAsync( query.AccessToken, ct );
    }
}
