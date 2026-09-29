using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Boundless.Desktop.Commands.Swapper;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels;

namespace Boundless.Desktop.UserControls.Popups;

public partial class SwapControlPopup : UserControl
{
    public CosmeticItemModel SwapTo { get; }
    public CosmeticItemModel? SwapFrom { get; }

    public SwapParameter Parameter => new(this, SwapFrom, SwapTo);
    
    public SwapControlPopup(CosmeticItemModel swapTo, CosmeticItemModel? swapFrom = null)
    {
        SwapTo = swapTo;
        SwapFrom = swapFrom;
        DataContext = Parameter;
        
        InitializeComponent();
        InitializeSwapControl();
        
        MainWindow.PopupContainer.AddPopup(this, true);
    }

    public void InitializeSwapControl()
    {
        if (SwapTo.Data?.Images == null)
        {
            var error = new NullReferenceException($"Images were null for {SwapTo.CosmeticId}");
            _ = new ErrorPopup("SwapControlPopup",
                $"Unable to load image for {SwapTo.CosmeticId}. If this error persists please report it to the dev team.",
                error);
        }
            
        SwapToImage.Source = new BitmapImage(new Uri(SwapTo.Data.Images.Icon));
        
        if (SwapFrom?.Data?.Images.Icon != null)
            SwapFromImage.Source = new BitmapImage(new Uri(SwapFrom.Data.Images.Icon));
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        MainWindow.PopupContainer.RemovePopup(this);
    }

    public void LogMessage(string message)
    {
        Log("SwapControlPopup", LogLevel.Info, message);
        LogBox.Text += message + "\n";
    }
}