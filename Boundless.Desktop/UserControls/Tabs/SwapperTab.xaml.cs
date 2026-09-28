using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Games;
using Boundless.Desktop.Games.EpicGames;
using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Swapper;
using Visibility = System.Windows.Visibility;

namespace Boundless.Desktop.UserControls.Tabs;

public partial class SwapperTab : UserControl
{
    public SwapperCore? Core { get; private set; }
    
    public SwapperTab()
    {
        InitializeComponent();
    }

    public async Task LoadSwapper()
    {
        if (Core != null)
            return;
        
        if (!EpicInstallations.Installs.TryGetValue("Fortnite", out var install))
        {
            throw new ApplicationException("Fortnite is not installed or the install has not been populated.");
        }
        
        Core = new SwapperCore(install.Path);
        Core.Initialize();
        UpdateCosmeticsLoadingProgress(.15);
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
                SwapperPanel.Visibility = Visibility.Visible;
            }
        });
    }
}