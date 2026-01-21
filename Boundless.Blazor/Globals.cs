global using static Boundless.Blazor.Globals;
using Boundless.Blazor.Config;
using RestSharp;

namespace Boundless.Blazor;

public static class Globals
{
    public const string VERSION = "1.0.0";

    public const string BACKEND_URI = "http://localhost:5197";
    public static RestClient Client = new(BACKEND_URI);
    
    public static void StartupTasks()
    {
        Logging.StartLogging(Configuration.LoggingPath);
        var now = DateTime.Now;
        Logger.Information($"Opened log file ({now.ToLongDateString()} - {now.ToLongTimeString()}");
        
        Configuration.Initialize();
    }
}