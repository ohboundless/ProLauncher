using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Boundless.Desktop.Config;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite;
using Boundless.Desktop.Templates;
using Boundless.Desktop.UserControls;
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

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadAccounts();
    }

    private async void LoadAccounts()
    {
        AccountsPanel.Children.Clear();

        await Accounts.Fortnite.LoadAsync();
        var accounts = Accounts.Fortnite.Accounts;
        if (accounts == null || accounts.Count == 0)
        {
            NoAccountsFoundTextBlock.Visibility = Visibility.Visible;
            return;
        }

        NoAccountsFoundTextBlock.Visibility = Visibility.Collapsed;
        
        foreach (var account in accounts)
        {
            var entry = new AccountEntry(account);
            AccountsPanel.Children.Add(entry);
        }
    }

    private async void LoadAccount_OnClick(object sender, RoutedEventArgs e)
    {
        await FortniteHandler.StartAuthorization();
        LoadAccounts();
    }
}