using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Boundless.Desktop.Config;
using Boundless.Desktop.Games.EpicGames.Fortnite;
using Boundless.Desktop.Templates;
using Boundless.Desktop.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Wpf;

namespace Boundless.Desktop.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : BoundlessWindow
{
    private UserControl _currentTab;

    public UserControl CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab == value)
                return;
            
            _currentTab = value;
            SetCurrentTab(value);
        }
    }
    
    public MainWindow()
    {
        Globals.MainWindow = this;
        InitializeComponent();
        
        SetCurrentTab(HomeTab);
    }

    public void SetCurrentTab(UserControl control)
    {
        // Make sure it's collapsed before we start
        control.Visibility = Visibility.Collapsed;
        
        if (!Tabs.Children.Contains(control))
            Tabs.Children.Add(control);

        foreach (var tab in Tabs.Children)
        {
            var ctl = (UserControl)tab;
            ctl.Visibility = Visibility.Collapsed;
        }

        control.Visibility = Visibility.Visible;
    }

    public void SetCurrentTab<T>(Action? backAction = null) where T : UserControl
    {
        foreach (var tab in Tabs.Children)
        {
            if (tab.GetType() != typeof(T)) 
                continue;
            
            SetCurrentTab((UserControl)tab);
            return;
        }

        var instance = backAction == null
            ? Activator.CreateInstance<T>()
            : Activator.CreateInstance(typeof(T), backAction);
        
        if (instance is not UserControl userControl)
            throw new InvalidCastException("Tab must be a user control!");
        
        SetCurrentTab(userControl);
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        
    }
    
    private void Home_OnClick(object sender, MouseButtonEventArgs e)
    {
        SetCurrentTab(HomeTab);
    }

    private void Library_OnClick(object sender, MouseButtonEventArgs e)
    {
        SetCurrentTab(LibraryTab);
    }
}