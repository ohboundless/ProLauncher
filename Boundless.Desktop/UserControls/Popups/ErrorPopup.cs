using System.Windows;
using System.Windows.Controls;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class ErrorPopup : MessagePopup
{
    public ErrorPopup(string category, string message, Exception exception) : base(MainWindow.PopupContainer,
        "ERROR",
        message,
        "Copy and close",
        () => Clipboard.SetText(exception.ToString()))
    {
        InitializeComponent();
        Messages.ShowMessage(this);
        Log(category + "_ErrorPopup", LogLevel.Error, exception.ToString());
    }
}