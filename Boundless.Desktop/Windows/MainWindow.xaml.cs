using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Commands;
using Boundless.Desktop.Config;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;
using Boundless.Desktop.Templates;
using Boundless.Desktop.UserControls;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace Boundless.Desktop.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : BoundlessWindow
{
    public FortniteAccount? ActiveAccount { get; private set; }
    
    public MainWindow()
    {
        Globals.MainWindow = this;
        InitializeComponent();

        DataContext = this;
        
        // Loops
        GameProcessList.FortniteProcessLoop();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadAccounts();
    }

    public async void LoadAccounts()
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

        if (accountIds.TryGetValue("Fortnite", out var currentAccount))
        {
            // If the current account doesn't exist
            if (ordered.FirstOrDefault(x => x.Id == currentAccount) == null)
            {
                currentAccount = null;
                accountIds.Remove("Fortnite");
            }
        }
        
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

        var activeAccount = accounts.First(x => x.Id == currentAccount);
        OnSetActiveAccount(activeAccount);
    }

    public void OnSetActiveAccount(FortniteAccount account, bool reloadData = false)
    {
        ActiveAccount = account;
        AccountCosmeticsTab.OnSetActiveAccount(account, reloadData);
    }

    private void LaunchFortnite_OnClick(object sender, RoutedEventArgs e)
    {
        StaticCommands.LaunchFortniteCommand.Execute(null);
    }
    
    private void AddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        StaticCommands.AddAccountCommand.Execute(null);
    }

    private void CloseFortnite_OnClick(object sender, RoutedEventArgs e)
    {
        GameProcessList.CloseFortnite();
    }
}