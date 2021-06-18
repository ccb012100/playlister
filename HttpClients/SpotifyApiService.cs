using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.HttpClients
{
    public class SpotifyApiService
    {
        private SpotifyOptions _options;

        public HttpClient Client { get; }

        public SpotifyApiService(HttpClient client, IOptions<SpotifyOptions> options)
        {
            Client = client;
            _options = options.Value;
        }

        /// <summary>
        /// Get Playlist Items for specified <param name="playlistId"></param>.
        /// </summary>
        /// <param name="playlistId">Spotify Id of the playlist</param>
        /// <param name="offset"></param>
        /// <param name="limit">Number of items to return</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PagingObject<PlaylistItem>> GetPlaylistItems(string playlistId, int? offset, int? limit,
            CancellationToken ct)
        {
            var queryParams = new Dictionary<string, string?>
            {
                {"market", "from_token"},
                {
                    "fields",
                    "fields=limit,url,previous,offset,limit,total,href," +
                    "items(added_at,track(id,track_number,disc_number,duration_ms,name,artists(id,name),album(name,id,release_date,total_tracks,album_type,artists(id,name))))"
                }
            };

            if (offset.HasValue)
            {
                queryParams.Add("offset", offset.Value.ToString());
            }

            if (limit.HasValue)
            {
                queryParams.Add("limit", limit.Value.ToString());
            }

            string? uri = Url.Combine(_options.ApiBaseAddress.ToString(), $"playlists/{playlistId}/tracks");
            string fullUrl = QueryHelpers.AddQueryString(uri, queryParams);

            return await GetPlaylistItemsAsync(new Uri(fullUrl), ct);
        }

        public async Task<PagingObject<PlaylistItem>> GetPlaylistItems(Uri next, CancellationToken ct) =>
            await GetPlaylistItemsAsync(next, ct);

        private async Task<PagingObject<PlaylistItem>> GetPlaylistItemsAsync(Uri url, CancellationToken ct) =>
            (await Client.GetFromJsonAsync<PagingObject<PlaylistItem>>(url, cancellationToken: ct))!;
    }
}
