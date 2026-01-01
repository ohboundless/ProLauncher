using System.Windows;
using Serilog;

namespace Boundless.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Logging.StartLogging(Configuration.LoggingPath);

        var now = DateTime.Now;
        Logger.Information($"Opened log file ({now.ToLongDateString()} - {now.ToLongTimeString()}");
        
        Logger.Information("Starting blazor application...");
        // Start blazor app
        Logger.Information("Launched blazor application. (not yet)");
        
        base.OnStartup(e);
    }
}