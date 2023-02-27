using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Playlister.Middleware
{
    /// <summary>
    /// Logs HTTP requests.
    /// Use on RefitApi registration to see Request/Response info for easier debugging.
    /// Adapted from https://github.com/reactiveui/refit/issues/258#issuecomment-243394076
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HttpLoggingMiddleware : DelegatingHandler
    {
        private readonly ILogger<HttpLoggingMiddleware> _logger;

        public HttpLoggingMiddleware(ILogger<HttpLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken ct
        )
        {
            string? msg;
            HttpResponseMessage? response,
                resp;

            using (HttpRequestMessage req = request)
            {
                var id = Guid.NewGuid().ToString();
                msg = $"[{id} -  Request]";

                _logger.LogDebug("{Msg}========Start==========", msg);
                _logger.LogDebug(
                    "{Msg} {Method} {PathAndQuery} {Scheme}/{Version}",
                    msg,
                    req.Method,
                    req.RequestUri!.PathAndQuery,
                    req.RequestUri.Scheme,
                    req.Version
                );
                _logger.LogDebug(
                    "{Msg} Host: {Scheme}://{Host}",
                    msg,
                    req.RequestUri.Scheme,
                    req.RequestUri.Host
                );

                foreach (var (key, value) in req.Headers)
                {
                    _logger.LogDebug(
                        "{Msg} {HeaderKey}: {HeaderValue}",
                        msg,
                        key,
                        string.Join(", ", value)
                    );
                }

                if (req.Content is not null)
                {
                    foreach (var (key, value) in req.Content.Headers)
                    {
                        _logger.LogDebug(
                            "{Msg} {HeaderKey}: {HeaderValue}",
                            msg,
                            key,
                            string.Join(", ", value)
                        );
                    }

                    if (
                        req.Content is StringContent
                        || IsTextBasedContentType(req.Headers)
                        || IsTextBasedContentType(req.Content.Headers)
                    )
                    {
                        string result = await req.Content.ReadAsStringAsync(ct);

                        _logger.LogDebug("{Msg} Content:", msg);
                        _logger.LogDebug("{Msg} {Result}", msg, result);
                    }
                }

                DateTime start = DateTime.Now;

                response = await base.SendAsync(request, ct).ConfigureAwait(false);

                DateTime end = DateTime.Now;

                _logger.LogDebug("{Msg} Duration: {Duration}", msg, end - start);
                _logger.LogDebug("{Msg}==========End==========", msg);

                msg = $"[{id} - Response]";
                _logger.LogDebug("{Msg}=========Start=========", msg);

                resp = response;

                _logger.LogDebug(
                    "{Msg} {scheme}/{version} {StatusCode} {ReasonPhrase}",
                    msg,
                    req.RequestUri.Scheme.ToUpper(),
                    resp.Version,
                    (int)resp.StatusCode,
                    resp.ReasonPhrase
                );
            }

            foreach (var (key, value) in resp.Headers)
            {
                _logger.LogDebug(
                    "{Msg} {HeaderKey}: {HeaderValue}",
                    msg,
                    key,
                    string.Join(", ", value)
                );
            }

            foreach (var (key, value) in resp.Content.Headers)
            {
                _logger.LogDebug(
                    "{Msg} {HeaderKey}: {HeaderValue}",
                    msg,
                    key,
                    string.Join(", ", value)
                );
            }

            if (
                resp.Content is StringContent
                || IsTextBasedContentType(resp.Headers)
                || IsTextBasedContentType(resp.Content.Headers)
            )
            {
                DateTime start = DateTime.Now;
                string result = await resp.Content.ReadAsStringAsync(ct);
                DateTime end = DateTime.Now;

                _logger.LogDebug("{Msg} Content:", msg);
                _logger.LogDebug("{Msg} {Result}...", msg, result);
                _logger.LogDebug("{Msg} Duration: {Duration}", msg, end - start);
            }

            _logger.LogDebug("{Msg}==========End==========", msg);
            return response;
        }

        private readonly string[] _types =
        {
            "html",
            "text",
            "xml",
            "json",
            "txt",
            "x-www-form-urlencoded"
        };

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out IEnumerable<string>? values))
            {
                return false;
            }

            string header = string.Join(" ", values).ToLowerInvariant();

            return _types.Any(t => header.Contains(t));
        }
    }
}
