using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Microsoft.AspNetCore.WebUtilities;
using Playlister.Utilities;
using Polly;

namespace Playlister.Extensions
{
    public static class HttpClientExtensions
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Handle 429 Request Limit Exceeded
            return Policy
                .Handle<Refit.ApiException>(x => x.StatusCode == HttpStatusCode.TooManyRequests)
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3,
                    sleepDurationProvider: (retryCount, response, context) =>
                    {
                        // taking the pseudocode from the question
                        var serverWaitDuration =
                            response.Result?.Headers.Get("Retry-After");
                        return TimeSpan.FromMilliseconds(serverWaitDuration);
                    },
                    onRetryAsync: async (response, timespan, retryCount, context) =>
                    {
                        /* perhaps some logging, eg the retry count, the timespan delaying for */
                    }
                );
        }
    }
}
