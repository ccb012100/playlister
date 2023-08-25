using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
                        onRetryAsync: async (_, timespan, retryAttempt, _) =>
                        {
                            svc.GetService<ILogger<SpotifyApiService>>()?.LogWarning(
                                "Received a 429 HTTP response; delaying for {TotalSeconds} seconds, then making retry attempt {AttemptCount}", timespan.TotalSeconds, retryAttempt);
                            
                            await Task.CompletedTask;
                        }
                    );
    }
}
