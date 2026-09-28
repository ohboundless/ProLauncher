using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels;
using Boundless.Swapper;

namespace Boundless.Desktop.UserControls;

public class SwapperPanel : CosmeticPanel
{
    public SwapperPanel()
    {
        CosmeticsViewModel = new SwapperCosmeticsViewModel();
        DataContext = CosmeticsViewModel;
    }
}