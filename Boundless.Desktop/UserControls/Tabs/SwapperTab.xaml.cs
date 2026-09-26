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
    public SwapperCore Core { get; private set; }
    private CosmeticTab _cosmeticTab { get; }
    
    public SwapperTab()
    {
        InitializeComponent();
        CosmeticsPanel.Children.Add(_cosmeticTab = new CosmeticTab(isSwapper: true));
    }

    public async Task LoadSwapper()
    {
        if (!EpicInstallations.Installs.TryGetValue("Fortnite", out var install))
        {
            throw new ApplicationException("Fortnite is not installed or the install has not been populated.");
        }
        
        Core = new SwapperCore(install.Path);
        await Core.Initialize();
        UpdateCosmeticsLoadingProgress(.25); // Provider loaded
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
                _cosmeticTab.Visibility = Visibility.Visible;
            }
        });
    }
}