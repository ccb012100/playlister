using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Extensions;
using Playlister.Models;
using Playlister.Models.SpotifyApi;

namespace Playlister.HttpClients
{
    public class SpotifyApiService
    {
        private readonly Uri _apiBaseAddress;

        private HttpClient Client { get; }

        public SpotifyApiService(HttpClient client, IOptions<SpotifyOptions> options)
        {
            Client = client;
            _apiBaseAddress = options.Value.ApiBaseAddress;
        }

        #region GetPlaylistItems

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
            Dictionary<string, string?> qp = GetQueryParams(offset, limit);

            qp.Add("fields",
                "fields=limit,url,previous,offset,limit,total,href," +
                "items(added_at,track(id,track_number,disc_number,duration_ms,name,artists(id,name),album(name,id,release_date,total_tracks,album_type,artists(id,name))))");

            return await Client.GetSpotifyDataFromJsonAsync<PagingObject<PlaylistItem>>(
                _apiBaseAddress, $"playlists/{playlistId}/tracks", qp, ct);
        }

        public async Task<PagingObject<PlaylistItem>> GetPlaylistItems(Uri next, CancellationToken ct) =>
            (await Client.GetFromJsonAsync<PagingObject<PlaylistItem>>(next, ct))!;

        #endregion

        #region GetCurrentUserPlaylists

        /// <summary>
        /// Get the current user's playlists.
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="offset"></param>
        /// <param name="limit">Number of items to return.</param>
        /// <returns></returns>
        public async Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylists(CancellationToken ct,
            int? offset = null,
            int? limit = 50)
        {
            return await Client.GetSpotifyDataFromJsonAsync<PagingObject<SimplifiedPlaylistObject>>(
                _apiBaseAddress, $"me/playlists?market=from_token", GetQueryParams(offset, limit), ct);
        }

        /// <summary>
        /// Get the current user's playlists.
        /// </summary>
        /// <param name="next">URI of the next page of playlists</param>
        /// <param name="ct"></param>
        /// <returns>A <c>PagingObject</c> containing the playlists requested in the <paramref name="next"/> Uri parameter</returns>
        public async Task<PagingObject<SimplifiedPlaylistObject>> GetCurrentUserPlaylists(Uri next,
            CancellationToken ct) => (await Client.GetFromJsonAsync<PagingObject<SimplifiedPlaylistObject>>(next, ct))!;

        #endregion

        private static Dictionary<string, string?> GetQueryParams(int? offset, int? limit)
        {
            var queryParams = new Dictionary<string, string?>
            {
                {"market", "from_token"},
            };

            if (offset.HasValue)
            {
                queryParams.Add("offset", offset.Value.ToString());
            }

            if (limit.HasValue)
            {
                queryParams.Add("limit", limit.Value.ToString());
            }

            return queryParams;
        }
    }
}
