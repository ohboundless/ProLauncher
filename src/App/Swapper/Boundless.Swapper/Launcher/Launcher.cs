using System.Diagnostics;

namespace Boundless.Swapper.Launcher;

public static class Launcher
{
    public static Process? Launch()
    {
        var config = new LaunchConfig();
        return Process.Start(new ProcessStartInfo
        {
            FileName = config.ExePath,
            WorkingDirectory = LaunchConfig.LauncherDirectory,
            Arguments = $"\"{config.EditsPath}\""
        });
    }
}