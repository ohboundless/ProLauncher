using System.Windows;

namespace Boundless.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        Logging.StartLogging(Configuration.LoggingPath);

        var now = DateTime.Now;
        Logger.Information($"Opened log file ({now.ToLongDateString()} - {now.ToLongTimeString()}");
    }
}