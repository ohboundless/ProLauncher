using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Boundless.Desktop.API;
using Boundless.Desktop.Config;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.Games.EpicGames.Fortnite;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Templates;
using Boundless.Desktop.UserControls;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : BoundlessWindow
{
    private FortniteAccount? _activeAccount;
    private CosmeticTab? _cosmeticTab;
    private CancellationTokenSource? _currentCancellationSource;
    
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

        var activeAccount = accounts.First(x => x.Id == currentAccount);
        OnSetActiveAccount(activeAccount);
    }

    public async void OnSetActiveAccount(FortniteAccount account, bool reloadData = false)
    {
        if (account.ProfileData == null || reloadData)
        {
            account.ProfileData = await new FortniteProfile(account).LoadProfileData();
            await Accounts.Fortnite.SaveAsync();
        }
        
        AccountUsername.Text = account.Username; // TODO get actual player name
        EpicUsername.Text = account.Username; // TODO get actual epic name
        AccountIcon.Source = new BitmapImage(new Uri(account.Icon)); // TODO get account icon as currently equipped skin
        AccountLevel.Text = account.ProfileData.AccountLevel.ToString("N0");
        // TODO has upgraded BattlePass?
        BattlePassLevel.Text = account.ProfileData.BattlePassLevel.ToString("N0");
        CurrencyQuantity.Text = account.ProfileData.Currency.ToString("N0");
        _activeAccount = account;

        LoadCosmetics();
    }

    private async void AddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await FortniteHandler.StartAuthorization();
        }
        catch (Exception ex)
        {
            // TODO handle error
            Log("Accounts", LogLevel.Error, "Failed to authorize account!");
            Log("Accounts", LogLevel.Error, ex.ToString());
        }

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

    private CancellationTokenSource HandleCancellation()
    {
        if (_currentCancellationSource != null)
        {
            Log("MainWindow", LogLevel.Info, "Requesting cancellation...");
            _currentCancellationSource.Cancel();
            _currentCancellationSource = null;
        }
        
        return _currentCancellationSource = new CancellationTokenSource();
    }

    private void TopBarRadioButton_OnChecked(object sender, RoutedEventArgs e)
    {
        var s = sender as RadioButton;
        var name = s?.Content?.ToString()?.CapitalizeWord();
        
        if (name == null)
            return;
        
        foreach (CosmeticTab tab in Tabs.Children)
        {
            if (tab.Name != name)
            {
                tab.Visibility = Visibility.Collapsed;
                tab.Panel.Children.Clear(); // Clear cosmetics
                continue;
            }

            _cosmeticTab = tab;
            tab.Visibility = Visibility.Visible;
        }
        
        LoadCosmetics();
    }

    private void LoadCosmetics()
    {
        if (_cosmeticTab == null || _activeAccount == null)
            return;

        var athenaType = _cosmeticTab.Name switch
        {
            "Cosmetics" => "*", // All
            "Outfits" => "AthenaCharacter",
            "Backblings" => "AthenaBackpack",
            "Pickaxes" => "AthenaPickaxe",
            "Emotes" => "AthenaDance",
            "Gliders" => "AthenaGlider",
            "Wraps" => "AthenaItemWrap",
            _ => throw new ArgumentOutOfRangeException()
        };

        _cosmeticTab.Panel.Children.Clear(); // Clear cosmetics
        var myCosmetics = _activeAccount.ProfileData?.Cosmetics;
        if (myCosmetics == null)
            throw new NoNullAllowedException("Profile data was null.");

        FortniteApiCache.Load();
        var cancellationToken = HandleCancellation().Token;

        if (athenaType == "*") // All
        {
            _cosmeticTab.ProcessCosmeticsAsync(myCosmetics, cancellationToken);
        }
        else
        {
            var cosmeticsToLoad =
                myCosmetics.Where(cosmetic => cosmetic.TemplateId.StartsWith(athenaType)).ToList();
            _cosmeticTab.ProcessCosmeticsAsync(cosmeticsToLoad, cancellationToken);
        }

        // Clear cache
        FortniteApiCache.Save();
        FortniteApiCache.Cosmetics.Clear();
    }
}