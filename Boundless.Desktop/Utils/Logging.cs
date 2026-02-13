global using static Serilog.Log;
using Serilog;

namespace Boundless.Desktop.Utils;

public static class Logging
{
    public static void StartLogging(string path)
    {
        Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(path, 
                rollingInterval: RollingInterval.Hour, 
                rollOnFileSizeLimit: true)
            .CreateLogger();
    }
}