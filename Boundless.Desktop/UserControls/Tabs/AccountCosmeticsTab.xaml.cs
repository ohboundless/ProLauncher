using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Boundless.Desktop.API;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;
using Visibility = System.Windows.Visibility;

namespace Boundless.Desktop.UserControls.Tabs;

public partial class AccountCosmeticsTab : UserControl
{
    public AccountCosmeticsTab()
    {
        InitializeComponent();
    }

    public async Task LoadCosmetics()
    {
        await FortniteCache.GetAllCosmetics();
        Log("AccountCosmeticsTab", LogLevel.Info, "Downloaded cosmetics.");
    }

    public async void OnSetActiveAccount(FortniteAccount account, bool reloadData = false)
    {
        try
        {
            if (account.ProfileData == null || reloadData)
            {
                account.ProfileData = await new FortniteProfile(account).LoadProfileData();
                await Accounts.Fortnite.SaveAsync();
            }
        }
        catch (Exception ex)
        {
            new ErrorPopup("MainWindow_OnSetActiveAccount", $"Unable to load profile data for {account.Username}.\nIf this issue persists please report this to the dev team.", ex);
            return;
        }

        AccountUsername.Text = account.Username; // TODO get actual player name
        EpicUsername.Text = account.Username; // TODO get actual epic name
        AccountIcon.Source = new BitmapImage(new Uri(account.Icon)); // TODO get account icon as currently equipped skin
        AccountLevel.Text = account.ProfileData.AccountLevel.ToString("N0");
        // TODO has upgraded BattlePass?
        BattlePassLevel.Text = account.ProfileData.BattlePassLevel.ToString("N0");
        CurrencyQuantity.Text = account.ProfileData.Currency.ToString("N0");
        
        CosmeticPanel.CosmeticsViewModel?.OnTabSwitched();
    }
    
    public void UpdateCosmeticsLoadingProgress(double progress)
    {
        var percentage = progress * 100.0;
        Dispatcher.InvokeAsync(() =>
        {
            CosmeticProgressRing.Progress = percentage;
            
            if (percentage >= 100.0)
            {
                CosmeticProgressRing.Visibility = Visibility.Collapsed;
                CosmeticPanel.Visibility = Visibility.Visible;
            }
            else
            {
                CosmeticPanel.Visibility = Visibility.Collapsed;
                CosmeticProgressRing.Visibility = Visibility.Visible;
            }
        });
    }
}