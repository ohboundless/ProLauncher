global using static Boundless.Desktop.Utils.Logging;
using static Serilog.Log;
using Serilog;

namespace Boundless.Desktop.Utils;

public enum ELogLevel
{
    Verbose,
    Info,
    Warn,
    Error,
    Fatal
}

public interface ILog
{
    internal LogCategory Category { get; }
}

public sealed class LogCategory(string name)
{
    public string Name = name;

    public override string ToString()
    {
        return Name;
    }
}

public enum ELogCategory
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

    public static void Log(string category, ELogLevel level, string message)
    {
        var str = $"[{category}] {message}";
        switch (level)
        {
            case ELogLevel.Verbose:
                Logger.Verbose(str);
                break;
            case ELogLevel.Info:
                Logger.Information(str);
                break;
            case ELogLevel.Warn:
                Logger.Warning(str);
                break;
            case ELogLevel.Error:
                Logger.Error(str);
                break;
            case ELogLevel.Fatal:
                Logger.Fatal(str);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
    
    public static void Log(ELogCategory category, ELogLevel level, string message)
    {
        Log(category.ToString(), level, message);
    }
    
    public static void Log(LogCategory category, ELogLevel level, string message)
    {
        Log(category.Name, level, message);
    }
    
    public static void Log(ILog log, ELogLevel level, string message)
    {
        Log(log.Category.Name, level, message);
    }
}