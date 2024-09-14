using Playlister.CQRS.Queries;
using Playlister.Models.SpotifyApi;
using Playlister.RefitClients;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

public class CurrentUserHandler( ISpotifyApi api, IPlaylistService playlistService )
{
    private readonly ISpotifyApi _api = api;
    private readonly IPlaylistService _playlistService = playlistService;

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
