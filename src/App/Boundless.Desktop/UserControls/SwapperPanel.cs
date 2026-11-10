using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels;
using Boundless.Desktop.ViewModels.Enums;
using Boundless.Swapper;

namespace Boundless.Desktop.UserControls;

public class SwapperPanel : CosmeticPanel
{
    public SwapperPanel()
    {
        CosmeticsViewModel = new CosmeticsViewModel
        {
            CosmeticPanelType = PanelType.Swapper
        };

        DataContext = CosmeticsViewModel;
    }
}