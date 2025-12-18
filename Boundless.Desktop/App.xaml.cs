using System.Windows;

namespace Boundless.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        // do some code stuff like initializing your ViewModel or something else


        // Instanciate the view you want to display and show it
        MainWindow mainWindow = new MainWindow();
        mainWindow.ShowDialog();
    }
}