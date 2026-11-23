using System.Windows.Controls;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class WaitingForLoginPopup : MessagePopup
{
    public WaitingForLoginPopup(Action cancelAction) : base( 
        "Login Pending", 
        "Waiting for login to complete...", 
        "Cancel", 
        cancelAction,
        MainWindow?.PopupContainer)
    {
        InitializeComponent();
        Messages.ShowMessage(this);
    }
}