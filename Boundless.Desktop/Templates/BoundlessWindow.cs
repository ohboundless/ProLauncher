using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Boundless.Desktop.Templates;

public class BoundlessWindow : Window
{
    public BoundlessWindow()
    {
        WindowStyle = WindowStyle.None;
        BorderThickness = new Thickness(1);
        UseLayoutRounding = true;
        ResizeMode = ResizeMode.NoResize;
        Loaded += OnLoaded;
    }
    
    static BoundlessWindow()
    {
        // This makes sure it picks up the default style in XAML
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(BoundlessWindow),
            new FrameworkPropertyMetadata(typeof(BoundlessWindow))
        );
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var topBar = Template.FindName("TopBar", this) as Grid;
        if (topBar != null)
            topBar.MouseLeftButtonDown += TopBar_OnMouseLeftButtonDown;
        
        var minimizeBtn = Template.FindName("MinimizeBtn", this) as Button;
        if (minimizeBtn != null)
            minimizeBtn.Click += Minimize_OnClick;
        
        var maximizeBtn = Template.FindName("MaximizeBtn", this) as Button;
        if (maximizeBtn != null)
            maximizeBtn.Click += Maximize_OnClick;
        
        var exitBtn = Template.FindName("ExitBtn", this) as Button;
        if (exitBtn != null)
            exitBtn.Click += Exit_OnClick;
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

    private void Minimize_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    public (int Left, int Top, double Width, double Height) GetMontiorWorkInfo()
    {
        var mouseInWindow = Mouse.GetPosition(this);
        var mouseScreen = PointToScreen(mouseInWindow);
        POINT pt;
        pt.x = (int)mouseScreen.X;
        pt.y = (int)mouseScreen.Y;
        
        var monitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
        var mi = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
        GetMonitorInfo(monitor, ref mi);
        
        var dpiFactorX = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformFromDevice.M11 ?? 1;
        var dpiFactorY = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformFromDevice.M22 ?? 1;
        var workWidth = (mi.rcWork.right - mi.rcWork.left) * dpiFactorX;
        var workHeight = (mi.rcWork.bottom - mi.rcWork.top) * dpiFactorY;

        return (mi.rcWork.left, mi.rcWork.top, workWidth, workHeight);
    }

    private void SetMaxWindowSize()
    {
        var info = GetMontiorWorkInfo();
        MaxWidth = info.Width;
        MaxHeight = info.Height;
    }
    
    private void Maximize_OnClick(object sender, RoutedEventArgs e)
    {
        SetMaxWindowSize();
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
    
    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void RestoreFromMaximized(MouseButtonEventArgs e)
    {
        var mouseInWindow = e.GetPosition(this);
        var percentX = mouseInWindow.X / ActualWidth;

        // Save restore size
        var restoreWidth = RestoreBounds.Width;
        var restoreHeight = RestoreBounds.Height;

        var workInfo = GetMontiorWorkInfo();

        // Restore window first
        SystemCommands.RestoreWindow(this);

        // Calculate Left so cursor stays “attached”
        Left = workInfo.Left + (workInfo.Width * percentX) - (restoreWidth * percentX);
        Top = workInfo.Top; // anchored to top

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
        SetMaxWindowSize();
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
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