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
        try
        {
            SwapToImage.Source = new BitmapImage(SwapTo.IconUri);

            if (SwapFrom != null)
                SwapFromImage.Source = new BitmapImage(SwapFrom.IconUri);

        }
        catch (Exception ex)
        {
            ErrorPopup.Show("SwapControlPopup_InitializeSwapControl", "Unable to load icons! Please report this to the dev team!", ex);
        }
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        MainWindow.PopupContainer.RemovePopup(this);
    }

    public void LogMessage(string message)
    {
        Log("SwapControlPopup", LogLevel.Info, message);
        Dispatcher.InvokeAsync(() => { LogBox.Text += message + "\n"; });
    }
}