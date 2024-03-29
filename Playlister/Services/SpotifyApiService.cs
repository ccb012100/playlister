using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync(
            string accessToken,
            string playlistId,
            int? offset,
            int? limit,
            CancellationToken ct
        )
        {
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await _spotifyApi.GetPlaylistTracksAsync(
                accessToken,
                playlistId,
                offset,
                limit,
                ct
            );
        }

        public async Task<PagingObject<PlaylistItem>> GetPlaylistTracksAsync(
            string accessToken,
            Uri next,
            CancellationToken ct
        )
        {
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return (await Client.GetFromJsonAsync<PagingObject<PlaylistItem>>(next, ct))!;
        }

        public async Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync(
            string accessToken,
            CancellationToken ct,
            int? offset = null,
            int? limit = 50
        ) => await _spotifyApi.GetCurrentUserPlaylistsAsync(accessToken, offset, limit, ct);

        public async Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylistsAsync(
            string accessToken,
            Uri next,
            CancellationToken ct
        )
        {
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return (
                await Client.GetFromJsonAsync<PagingObject<SimplifiedPlaylistObject>>(next, ct)
            )!;
        }

        public async Task<SimplifiedPlaylistObject> GetPlaylistAsync(
            string accessToken,
            string playlistId,
            CancellationToken ct
        ) => await _spotifyApi.GetPlaylistAsync(accessToken, playlistId, ct);
    }
}
