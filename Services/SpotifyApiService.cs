using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Models.SpotifyApi;
using Playlister.RefitClients;

namespace Playlister.Services
{
    public class SpotifyApiService : ISpotifyApiService
    {
        private readonly ISpotifyApi _spotifyApi;

        private HttpClient Client { get; }

        public SpotifyApiService(HttpClient client, ISpotifyApi spotifyApi)
        {
            _spotifyApi = spotifyApi;
            Client = client;
        }

        public async Task<PagingObject<PlaylistItem>> GetPlaylistTracks(string playlistId, int? offset, int? limit,
            CancellationToken ct) =>
            await _spotifyApi.GetPlaylistTracks(playlistId, offset, limit, ct);

        public async Task<PagingObject<PlaylistItem>> GetPlaylistTracks(Uri next, CancellationToken ct) =>
            (await Client.GetFromJsonAsync<PagingObject<PlaylistItem>>(next, ct))!;

        public async Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylists(CancellationToken ct,
            int? offset = null, int? limit = 50) => await _spotifyApi.GetCurrentUserPlaylists(offset, limit, ct);

        public async Task<PagingObject<SimplifiedPlaylistObject>>
            GetCurrentUserPlaylists(Uri next, CancellationToken ct) =>
            (await Client.GetFromJsonAsync<PagingObject<SimplifiedPlaylistObject>>(next, ct))!;

        public async Task<SimplifiedPlaylistObject> GetPlaylist(string playlistId, CancellationToken ct) =>
            await _spotifyApi.GetPlaylist(playlistId, ct);
    }
}
