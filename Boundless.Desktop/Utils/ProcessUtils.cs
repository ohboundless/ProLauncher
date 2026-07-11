using System.Diagnostics;

namespace Boundless.Desktop.Utils;

public static class ProcessUtils
{
    public static Process? Start(this ProcessStartInfo info)
    {
        Log(LogCategory.Processes, LogLevel.Info, $"Launching {info.FileName} {info.Arguments}");
        return Process.Start(info);
    }
}