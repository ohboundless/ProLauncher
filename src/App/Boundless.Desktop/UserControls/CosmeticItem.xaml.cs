using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Boundless.Desktop.Enums;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.ViewModels;
using Boundless.Desktop.ViewModels.Enums;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticItem : UserControl
{
    public CosmeticItem()
    {
        InitializeComponent();
    }

    private async void CosmeticItem_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not CosmeticItemModel itemModel)
            throw new InvalidCastException("DataContext is not a CosmeticItemModel.");

        if (itemModel.PanelType == PanelType.AccountCosmetics)
            return;

        var filter = MainWindow!.SwapperTab.SwapperPanel.CosmeticsViewModel!.SelectedFilter;
        if (filter is CosmeticFilter.Pickaxes or CosmeticFilter.Emotes)
        {
            var panel = MainWindow.SelectSwapTab.CosmeticPanel;
            if (itemModel.PanelType == PanelType.Selecting)
            {
                _ = new SwapControlPopup(panel.SelectedSwap, itemModel);
                return;
            }
            
            MainWindow.SelectedTab = ActiveTab.SelectSwap;
            panel.SelectedSwap = itemModel;
            panel.CosmeticsViewModel.SelectedFilter = filter;
            panel.CosmeticsViewModel.SwapCosmeticId = itemModel.CosmeticId.ToLower();
            
            MainWindow.SelectSwapTab.OnSetActiveAccount(MainWindow.ActiveAccount!);
            return;
        }
            
        _ = new SwapControlPopup(itemModel);
    }

    private async void Image_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
            return;

        if (element.DataContext is not CosmeticItemModel item)
            return;

        try
        {
            await item.LoadAsync();
            
            if (item.SeriesUri != null)
                item.BackgroundRadial = new ImageBrush(new BitmapImage(item.SeriesUri));
        }
        catch (Exception ex)
        {
            Log("CosmeticLoading", LogLevel.Error, $"Failed to load cosmetic {item.CosmeticId}: {ex}");
        }
    }
}