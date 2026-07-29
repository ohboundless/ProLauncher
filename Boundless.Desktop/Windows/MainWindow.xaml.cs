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
    public MainWindow()
    {
        Globals.MainWindow = this;
        InitializeComponent();
    }
}