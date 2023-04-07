using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Playlister.Services;
using Polly;
using Refit;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleAnonymousFunction

namespace Playlister.Utilities
{
    public static class PollyUtility
    {
        // TODO: add policy for 401s and attempt to refresh token

        /// <summary>
        /// Policy for handling 429 response codes by waiting the time specified in the RetryAfter Header.
        /// </summary>
        public static readonly Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>>
            RetryAfterPolicy =
                (svc, _) => Policy
                    .Handle<ApiException>(x => x.StatusCode == HttpStatusCode.TooManyRequests)
                    .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: (_, response, _) =>
                            response.Result?.Headers.RetryAfter?.Delta ??
                            throw new InvalidOperationException("Could not find valid RetryAfter header."),
#pragma warning disable 1998
                        onRetryAsync: async (_, timespan, retryAttempt, _) =>
#pragma warning restore 1998
                        {
                            svc.GetService<ILogger<SpotifyApiService>>()?.LogWarning(
                                "Received a 429 HTTP response; delaying for {TotalSeconds} seconds, then making retry attempt {AttemptCount}", timespan.TotalSeconds, retryAttempt);
                        }
                    );
    }
}
