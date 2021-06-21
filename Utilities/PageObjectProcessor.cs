using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Playlister.Models.SpotifyApi;

namespace Playlister.Utilities
{
    public static class PageObjectProcessor
    {
        public static async Task ProcessPages<T>(
            Func<CancellationToken, Task<PagingObject<T>>> page1Fn,
            // Input: Uri and CancellationToken
            // Output: Task<PagingObject<T>>
            Func<Uri, CancellationToken, Task<PagingObject<T>>> page2PlusFn,
            // Input: T, CancellationToken
            // Output: void
            Action<IEnumerable<T>, CancellationToken> itemFn,
            CancellationToken ct)
        {
            // get 1st page
            PagingObject<T> page = await page1Fn.Invoke(ct);
            // process 1st page
            ProcessItems(page, itemFn, ct);

            while (page.Next is not null)
            {
                // get next page
                page = await page2PlusFn.Invoke(page.Next, ct);
                // process page
                ProcessItems(page, itemFn, ct);
            }
        }

        private static void ProcessItems<T>(PagingObject<T> page, Action<IEnumerable<T>, CancellationToken> itemFn,
            CancellationToken ct)
        {
            itemFn(page.Items, ct);
        }
    }
}
