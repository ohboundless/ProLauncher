using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels;
using Boundless.Desktop.ViewModels.Enums;
using Boundless.Swapper;

namespace Boundless.Desktop.UserControls;

public class SelectSwapPanel : CosmeticPanel
{
    // The currently selected item that we will swap to.
    // Used for the selection panel communicating with the swap control popup
    public CosmeticItemModel SelectedSwap { get; set; }
    
    public SelectSwapPanel()
    {
        CosmeticsViewModel = new CosmeticsViewModel
        {
            CosmeticPanelType = PanelType.Selecting
        };

        DataContext = CosmeticsViewModel;
    }
}