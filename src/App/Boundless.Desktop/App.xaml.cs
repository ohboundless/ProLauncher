using System.Data;
using System.Windows;
using System.Windows.Threading;
using Boundless.Desktop.UserControls.Popups;

namespace Boundless.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Errors
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        
        // Startup
        base.OnStartup(e);
        StartupTasks();
    }
    
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show("Unhandled exception! Report this to the dev team and provide your log file.");
        e.Handled = true;

        Log("App", LogLevel.Error, e.Exception.ToString());
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show("Unhandled exception! Report this to the dev team and provide your log file.");
            Log("App", LogLevel.Error, ex.ToString());
        }
    }
}