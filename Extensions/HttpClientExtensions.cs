using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.WebUtilities;
using Playlister.Utilities;

namespace Playlister.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetSpotifyDataFromJsonAsync<T>(this HttpClient client, Uri baseUrl,
            string relativePath, Dictionary<string, string?> queryParams, CancellationToken ct)
        {
            string uri = Url.Combine(baseUrl.ToString(), relativePath);
            Uri fullUrl = new(QueryHelpers.AddQueryString(uri, queryParams));

            return (await client.GetFromJsonAsync<T>(fullUrl, JsonUtility.SnakeCaseSerializerOptions, ct))!;
        }
    }
}
