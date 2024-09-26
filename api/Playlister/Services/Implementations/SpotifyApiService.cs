using System.Net.Http.Headers;

using Playlister.Models.SpotifyApi;
using Playlister.RefitClients;

namespace Playlister.Services.Implementations;

public class SpotifyApiService( HttpClient client , ISpotifyApi spotifyApi ) : ISpotifyApiService {
    private readonly HttpClient _client = client;
    private readonly ISpotifyApi _spotifyApi = spotifyApi;

    public async Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync(
        string accessToken ,
        string playlistId ,
        int? offset ,
        int? limit ,
        CancellationToken ct
    ) {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue( "Bearer" , accessToken );

        return await _spotifyApi.GetPlaylistTracksAsync(
            accessToken ,
            playlistId ,
            offset ,
            limit ,
            ct
        );
    }

    public async Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync(
        string accessToken ,
        Uri next ,
        CancellationToken ct
    ) {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue( "Bearer" , accessToken );

        return ( await _client.GetFromJsonAsync<PagingObject<PlaylistItem>>( next , ct ) )!;
    }

    public async Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync(
        string accessToken ,
        CancellationToken ct ,
        int? offset = null ,
        int? limit = 50
    ) {
        return await _spotifyApi.GetCurrentUserPlaylistsAsync( accessToken , offset , limit , ct );
    }

    public async Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync(
        string accessToken ,
        Uri next ,
        CancellationToken ct
    ) {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue( "Bearer" , accessToken );

        return (
            await _client.GetFromJsonAsync<PagingObject<SimplifiedPlaylistObject>>( next , ct )
        )!;
    }

    public async Task<SimplifiedPlaylistObject> GetPlaylistAsync(
        string accessToken ,
        string playlistId ,
        CancellationToken ct
    ) {
        return await _spotifyApi.GetPlaylistAsync( accessToken , playlistId , ct );
    }
}
