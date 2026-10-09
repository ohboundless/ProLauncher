using Serilog;
using static Serilog.Log;

namespace Boundless.Logging.Classes;

public enum LogLevel
{
    Verbose,
    Info,
    Warn,
    Error,
    Fatal
}

public enum LogCategory
{
    // Main
    Any,
    Http,
    Processes,
    UI,
    Startup,
    Files,
    Login
}

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

    public static void Log(string category, LogLevel level, string message)
    {
        var str = $"[{category}] {message}";
        switch (level)
        {
            case LogLevel.Verbose:
                Logger.Verbose(str);
                break;
            case LogLevel.Info:
                Logger.Information(str);
                break;
            case LogLevel.Warn:
                Logger.Warning(str);
                break;
            case LogLevel.Error:
                Logger.Error(str);
                break;
            case LogLevel.Fatal:
                Logger.Fatal(str);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }

    public static void Log(LogCategory category, LogLevel level, string message)
    {
        Log(category.ToString(), level, message);
    }
}