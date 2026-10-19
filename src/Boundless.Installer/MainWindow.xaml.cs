using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Boundless.Installer.Loader;

namespace Boundless.Installer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Statics.Log = WriteLog;
        Statics.Progress = Progress;
    }

    private async void WriteLog(string text)
    {
        await Dispatcher.InvokeAsync(() => LogBox.Text += text + "\n");
    }

    private async void Progress(double percentage)
    {
        await Dispatcher.InvokeAsync(() => ProgressRing.Progress = percentage);
    }

    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var installer = new Loader.Installer();
            await installer.Install();
            
            WriteLog("Launching...");
            Process.Start(new ProcessStartInfo
            {
                FileName = Statics.DirectoryPath + @"\app\Boundless.Desktop.exe"
            });
            
            Process.GetCurrentProcess().Kill();
        }
        catch (Exception ex)
        {
            LogBox.Text = string.Empty;
            WriteLog("An error has occurred! Please report this to the dev team!");
            WriteLog("An error has occurred! Please report this to the dev team!");
            WriteLog("An error has occurred! Please report this to the dev team!\n");
            WriteLog(ex.ToString());
        }
    }
}