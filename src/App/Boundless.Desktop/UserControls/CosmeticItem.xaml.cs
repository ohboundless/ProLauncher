using System.Windows.Controls;
using System.Windows.Input;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.ViewModels;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticItem : UserControl
{
    public CosmeticItem()
    {
        InitializeComponent();
    }

    private void CosmeticItem_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not CosmeticItemModel itemModel)
            throw new InvalidCastException("DataContext is not a CosmeticItemModel.");

        if (!itemModel.IsForSwapper)
            return;
            
        _ = new SwapControlPopup(itemModel);
    }
}