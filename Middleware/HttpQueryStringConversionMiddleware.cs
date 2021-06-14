using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Playlister.Extensions;

namespace Playlister.Middleware
{
    /// <summary>
    /// Http Handler that converts query string parameters to snake_casing.
    /// </summary>
    public class HttpQueryStringConversionMiddleware : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using HttpRequestMessage? req = request;

            if (!string.IsNullOrWhiteSpace(req.RequestUri!.Query))
            {
                Dictionary<string, StringValues>? queryDict =
                    Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.RequestUri!.Query);

                Dictionary<string, StringValues> snakeCaseQueryDict = new();

                foreach ((string name, StringValues value) in queryDict)
                {
                    snakeCaseQueryDict.Add(name.ToSnakeCase(), value);
                }

                string newUri =
                    Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(
                        req.RequestUri.GetLeftPart(UriPartial.Path), snakeCaseQueryDict);
                req.RequestUri = new Uri(newUri);
            }

            HttpResponseMessage? response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response;
        }
    }
}
