using System.Diagnostics;

namespace Boundless.Desktop.Utils;

public static class WebUtils
{
    public static void StartUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}