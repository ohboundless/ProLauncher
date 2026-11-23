using System.Windows;
using System.Windows.Controls;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class ErrorPopup : MessagePopup
{
    private ErrorPopup(string category, string message, Exception exception) : base(
        "ERROR",
        message,
        "Copy and close",
        () => Clipboard.SetText(exception.ToString()),
        MainWindow?.PopupContainer)
    {
        InitializeComponent();
        Messages.ShowMessage(this);
        Log(category + "_ErrorPopup", LogLevel.Error, exception.ToString());
    }

    public static void Show(string category, string message, Exception exception)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _ = new ErrorPopup(category, message, exception);
        });
    }
}