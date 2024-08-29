using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Playlister.Utilities
{
    public static class UrlUtility
    {
        // HACK: because of this => <https://github.com/dotnet/corefx/issues/10361>
        // ReSharper disable once UnusedMember.Global - not using it for now
        public static void OpenUrl(string url, ILogger logger)
        {
            string os = "other";

            try
            {
                if (RuntimeInformation.OSDescription.Contains("microsoft-standard-WSL2"))
                {
                    os = "WSL2";
                    Process.Start("wslview", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    os = "Linux";
                    Process.Start("open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    os = "OSX";
                    Process.Start("open", url);
                }
                else
                {
                    Process.Start(url);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error trying to open url {Url} on OS [{OS}]\nOS Description{OSDescription}", url, os,
                    RuntimeInformation.OSDescription);
            }
        }
    }
}
