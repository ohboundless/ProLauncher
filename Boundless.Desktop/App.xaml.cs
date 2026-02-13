using System.Windows;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        StartupTasks();
    }
}