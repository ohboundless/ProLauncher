using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Boundless.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        /*Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            WorkingDirectory = "B:\\Projects\\ProLauncher\\Boundless.Blazor\\bin\\Debug\\net9.0",
            Arguments = "/c dotnet Boundless.Blazor.dll"
        });*/
        
        InitializeComponent();

        WebView.Source = new Uri("http://localhost:5185/");
        WebView.Loaded += (_, _) =>
        {
            WebView.Visibility = Visibility.Visible;
            LoadingCanvas.Visibility = Visibility.Collapsed;
        };
    }

    private void TopBar_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}