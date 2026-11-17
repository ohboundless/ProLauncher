using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Enums;
using Boundless.Desktop.FortniteAPI;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;
using Boundless.Swapper.Cosmetics.Models;
using Visibility = System.Windows.Visibility;

namespace Boundless.Desktop.UserControls.Tabs;

public partial class SelectSwapTab : UserControl
{
    public SelectSwapTab()
    {
        InitializeComponent();
    }

    public async Task OnSetActiveAccount(FortniteAccount account, bool reloadData = false)
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
            ErrorPopup.Show("MainWindow_OnSetActiveAccount", 
                $"Unable to load profile data for {account.Username}.\nIf this issue persists please report this to the dev team.", 
                ex);
            return;
        }
        
        CosmeticPanel.CosmeticsViewModel?.OnTabSwitched();
    }
    
    public async void UpdateCosmeticsLoadingProgress(double progress)
    {
        var percentage = progress * 100.0;
        await Dispatcher.InvokeAsync(() =>
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

    private void Back_OnClick(object sender, RoutedEventArgs e)
    {
        MainWindow!.SelectedTab = ActiveTab.Swapper;
    }
}