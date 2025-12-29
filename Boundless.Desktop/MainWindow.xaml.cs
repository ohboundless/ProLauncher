using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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
        var mouseInWindow = e.GetPosition(this);
        var percentX = mouseInWindow.X / ActualWidth;

        // Save restore size
        var restoreWidth = RestoreBounds.Width;
        var restoreHeight = RestoreBounds.Height;

        // Convert mouse position to screen coordinates (DIPs -> pixels)
        var mouseScreen = PointToScreen(mouseInWindow);
        POINT pt;
        pt.x = (int)mouseScreen.X;
        pt.y = (int)mouseScreen.Y;

        // Get monitor under cursor
        var monitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
        var mi = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
        GetMonitorInfo(monitor, ref mi);

        // Get working area in DIPs
        var dpiFactorX = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformFromDevice.M11 ?? 1;
        var dpiFactorY = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformFromDevice.M22 ?? 1;

        var workLeft = mi.rcWork.left * dpiFactorX;
        var workTop = mi.rcWork.top * dpiFactorY;
        var workWidth = (mi.rcWork.right - mi.rcWork.left) * dpiFactorX;
        // var workHeight = (mi.rcWork.bottom - mi.rcWork.top) * dpiFactorY;

        // Restore window first
        SystemCommands.RestoreWindow(this);

        // Calculate Left so cursor stays “attached”
        Left = workLeft + (workWidth * percentX) - (restoreWidth * percentX);
        Top = workTop; // anchored to top

        Width = restoreWidth;
        Height = restoreHeight;

        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                DragMove();
            }
            catch { /* ignore if mouse released already */ }
        }), System.Windows.Threading.DispatcherPriority.Input);
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

    private void MainWindow_OnSourceInitialized(object? sender, EventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(hwnd)?.AddHook(WndProc);
    }
    
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_GETMINMAXINFO = 0x0024;

        if (msg == WM_GETMINMAXINFO)
        {
            WmGetMinMaxInfo(hwnd, lParam);
            handled = true;
        }

        return IntPtr.Zero;
    }
    
    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

        IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            GetMonitorInfo(monitor, ref monitorInfo);

            RECT workArea = monitorInfo.rcWork;
            RECT monitorArea = monitorInfo.rcMonitor;

            mmi.ptMaxPosition.x = Math.Abs(workArea.left - monitorArea.left);
            mmi.ptMaxPosition.y = Math.Abs(workArea.top - monitorArea.top);
            mmi.ptMaxSize.x = Math.Abs(workArea.right - workArea.left);
            mmi.ptMaxSize.y = Math.Abs(workArea.bottom - workArea.top);
        }

        Marshal.StructureToPtr(mmi, lParam, true);
    }
    
    private const int MONITOR_DEFAULTTONEAREST = 2;

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
    
    
    [DllImport("user32.dll")]
    static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
    
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public int dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}