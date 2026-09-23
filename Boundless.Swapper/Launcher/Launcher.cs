using System.Diagnostics;

namespace Boundless.Swapper.Launcher;

public static class Launcher
{
    private static LaunchConfig _launchConfig => SC.LaunchConfig;
    
    public static void Launch()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = _launchConfig.ExePath,
            Arguments = $"\"{_launchConfig.EditsPath}\""
        });
    }
}