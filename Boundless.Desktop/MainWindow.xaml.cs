using System.Windows;
using System.Windows.Input;
using Microsoft.Web.WebView2.Wpf;

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
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        MaxHeight = SystemParameters.WorkArea.Height;
        MaxWidth = SystemParameters.WorkArea.Width;
        
        WebView.CoreWebView2InitializationCompleted += (s, e) =>
        {
            var webView = (WebView2)s!;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            webView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            webView.CoreWebView2.Settings.IsReputationCheckingRequired = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = false;

            webView.Loaded += (_, _) =>
            {
                webView.Visibility = Visibility.Visible;
                LoadingCanvas.Visibility = Visibility.Collapsed;
            };

            webView.Source = new Uri("http://localhost:5185/");
        };
        
        WebView.EnsureCoreWebView2Async();
    }
    
    private void TopBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
                return;
            }

            if (WindowState == WindowState.Maximized)
            {
                RestoreFromMaximized(e);
            }

            DragMove();
        }
    }
    
    private void RestoreFromMaximized(MouseButtonEventArgs e)
    {
        var mousePos = e.GetPosition(this);
        var percentX = mousePos.X / ActualWidth;
        
        WindowState = WindowState.Normal;
        Left = SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width * percentX - RestoreBounds.Width * percentX;
        Top = SystemParameters.WorkArea.Top;
    }
    
    private void ToggleMaximize()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void Maximize_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void Minimize_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
}