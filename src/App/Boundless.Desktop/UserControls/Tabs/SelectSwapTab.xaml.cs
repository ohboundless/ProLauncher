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

    public void OnSetActiveAccount(FortniteAccount account, bool reloadData = false)
    {
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