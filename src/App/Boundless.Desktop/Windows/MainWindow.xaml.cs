using System.Windows;
using System.Windows.Input;
using Boundless.Desktop.Commands;
using Boundless.Desktop.Config;
using Boundless.Desktop.Enums;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.UserControls;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels;
using LobbyBotController;
using Newtonsoft.Json;

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

    private ActiveTab _selectedTab = ActiveTab.Cosmetics;
    public ActiveTab SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (_selectedTab == value)
                return;

            _selectedTab = value;
            switch (value)
            {
                case ActiveTab.Cosmetics:
                    SwapperTab.Visibility = Visibility.Collapsed;
                    AccountCosmeticsTab.Visibility = Visibility.Visible;
                    break;
                case ActiveTab.Swapper:
                    SwapperTab.Visibility = Visibility.Visible;
                    AccountCosmeticsTab.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadAccounts();
    }

    public async void LoadAccounts()
    {
        try
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
            await OnSetActiveAccount(activeAccount);
        }
        catch (Exception ex)
        {
            _ = new ErrorPopup("MainWindow_LoadAccounts",
                "Unable to load accounts! If this error persists please report it to the dev team!", ex);
        }
    }

    public async Task OnSetActiveAccount(FortniteAccount account, bool reloadData = false)
    {
        ActiveAccount = account;
        await AccountCosmeticsTab.OnSetActiveAccount(account, reloadData);
    }
    
    private void AddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        StaticCommands.AddAccountCommand.Execute(null);
    }

    private void LaunchFortnite_OnChecked(object sender, RoutedEventArgs e)
    {
        LaunchFortniteBtn.Visibility = Visibility.Collapsed;
        LaunchFortniteBtn.IsChecked = false;
        CloseFortniteBtn.Visibility = Visibility.Visible;
        
        StaticCommands.LaunchFortniteCommand.Execute(null);
    }

    private void CloseFortnite_MouseUp(object sender, RoutedEventArgs e)
    {
        LaunchFortniteBtn.Visibility = Visibility.Visible;
        CloseFortniteBtn.Visibility = Visibility.Collapsed;
        
        GameProcessList.CloseFortnite();
    }

    private async void CosmeticsTab_OnChecked(object sender, RoutedEventArgs e)
    {
        SelectedTab = ActiveTab.Cosmetics;
        await AccountCosmeticsTab.LoadCosmetics();
    }
    
    private void SwapperTab_OnChecked(object sender, RoutedEventArgs e)
    {
        SelectedTab = ActiveTab.Swapper;
        SwapperTab.LoadSwapper();
        SwapperTab.SwapperPanel?.CosmeticsViewModel?.SelectedFilter = CosmeticFilter.Outfits;
    }
    
    private async void LobbyBotTab_OnChecked(object sender, RoutedEventArgs e)
    {
        try
        {
            //SelectedTab = ActiveTab.LobbyBot;
            await LaunchFortniteCommand.StartLobbyBot();
        }
        catch (Exception ex)
        {
            ErrorPopup.Show("MainWindow_LobbyBotTab_OnChecked",
                "Could not start lobby bot! If this error persists please report it to the dev team!", ex);
        }
    }
    
    private void StatsTab_OnChecked(object sender, RoutedEventArgs e)
    {
        SelectedTab = ActiveTab.Stats;
    }
}