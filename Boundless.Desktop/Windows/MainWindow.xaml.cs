using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Boundless.Desktop.Config;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames;
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

        var ordered = accounts.OrderByDescending(x => x.LastLaunchTime);
        var accountIds = Configuration.ConfigData!.CurrentAccounts.AccountIdsByGame;
        accountIds.TryGetValue("Fortnite", out var currentAccount);

        foreach (var account in ordered)
        {
            var entry = new AccountEntry(account, AccountsPanel);
            if (currentAccount == null)
            {
                entry.IsActive = true;
                currentAccount = account.Id;
                accountIds.Add("Fortnite", account.Id);
            }
            else if (account.Id == currentAccount)
            {
                entry.IsActive = true;
            }
            
            AccountsPanel.Children.Add(entry);
        }
    }

    private async void LoadAccount_OnClick(object sender, RoutedEventArgs e)
    {
        await FortniteHandler.StartAuthorization();
        LoadAccounts();
    }

    private async void LaunchFortnite_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            EpicInstallations.PopulateInstallations(); // TODO probably put somewhere else

            if (!Configuration.ConfigData!.CurrentAccounts.AccountIdsByGame.TryGetValue("Fortnite", out var current))
            {
                Log("Launcher", LogLevel.Error, "Cannot launch Fortnite, no active account found!");
                // TODO maybe UI message
                return;
            }

            var account = Accounts.Fortnite.Accounts?.FirstOrDefault(x => x.Id == current);

            if (account == null)
            {
                Log("Launcher", LogLevel.Error, $"Unable to launch! Could not find account with id: {current}");
                // TODO UI message
                return;
            }
            
            await FortniteHandler.LaunchFortnite(account);
            LoadAccounts();
        }
        catch (Exception ex)
        {
            // TODO handle
            Log("Launcher", LogLevel.Error, ex.ToString());
        }
    }
}