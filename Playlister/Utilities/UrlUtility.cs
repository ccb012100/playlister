using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Playlister.Utilities
{
    public static class UrlUtility
    {
        public static void OpenUrl(string url)
        {
            // HACK: because of this => <https://github.com/dotnet/corefx/issues/10361>
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                try
                {
                    Process.Start(url);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Encountered an error trying to open the URL on Operating System '{RuntimeInformation.OSDescription}'", e);
                }
            }
        }
    }
}
