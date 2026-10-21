using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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