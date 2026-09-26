using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.UserControls.Popups;
using Visibility = System.Windows.Visibility;

namespace Boundless.Desktop.UserControls.Tabs;

public partial class SwapperTab : UserControl
{
    public SwapperTab()
    {
        InitializeComponent();
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
                CosmeticTab.Visibility = Visibility.Visible;
            }
        });
    }
}