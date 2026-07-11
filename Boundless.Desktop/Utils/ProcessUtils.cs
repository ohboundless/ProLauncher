using System.Diagnostics;

namespace Boundless.Desktop.Utils;

public static class ProcessUtils
{
    public static Process? Start(this ProcessStartInfo info)
    {
        Log(ELogCategory.Processes, ELogLevel.Info, $"Launching {info.FileName} {info.Arguments}");
        return Process.Start(info);
    }
}