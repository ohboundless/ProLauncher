global using static Boundless.Desktop.Utils.Globals;
using System.IO;
using System.Windows.Media.Media3D;
using Boundless.Desktop.Config;
using Boundless.Desktop.Windows;
using RestSharp;

namespace Boundless.Desktop.Utils;

public static class Globals
{
    public const string VERSION = "1.0.0";

    public const string BACKEND_URI = "http://localhost:5197";
    public static RestClient Client = new(BACKEND_URI);

    public static MainWindow MainWindow;
    
    public static void StartupTasks()
    {
        Logging.StartLogging(Configuration.LoggingPath);
        var now = DateTime.UtcNow;
        Logger.Information($"Opened log file ({now.ToLongDateString()} - {now.ToLongTimeString()})");
        
        Configuration.Initialize();
    }
}