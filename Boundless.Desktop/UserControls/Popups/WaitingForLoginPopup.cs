using System.Windows.Controls;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class WaitingForLoginPopup : MessagePopup
{
    public WaitingForLoginPopup(Action cancelAction) : base(MainWindow.PopupContainer, 
        "Login Pending", 
        "Waiting for login to complete...", 
        "Cancel", 
        cancelAction)
    {
        InitializeComponent();
        Messages.ShowMessage(this);
    }
}