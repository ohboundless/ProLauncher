using System.Windows.Controls;
using Boundless.Desktop.UserControls.Popups;

namespace Boundless.Desktop.Utils;

public static class Messages
{
    public static void ShowMessage(this PopupContainer parent, 
        string title, 
        string message, 
        string confirmText = "Ok", 
        Action? confirmAction = null, 
        bool removeAllPopups = true)
    {
        ShowMessage(
            new MessagePopup(title,
                message, 
                confirmText, 
                confirmAction, 
                parent), 
            removeAllPopups);
    }
    
    public static void ShowMessage(MessagePopup popupMessage, bool removeAllPopups = true)
    {
        Log("Messages", LogLevel.Info,
            $"Creating popup: Title: '{popupMessage.Title}'. Message: '{popupMessage.Message}'. ConfirmText: '{popupMessage.ConfirmButtonText}'. IsConfirmActionNull: {popupMessage.ConfirmAction == null}");
        popupMessage.ParentGrid!.AddPopup(popupMessage, removeAllPopups);
    }

    public static void QueueMessage(
        string title, 
        string message, 
        string confirmText = "Ok", 
        Action? confirmAction = null)
    {
        var popup = new MessagePopup(title, message, confirmText, confirmAction);
        QueuedMessage = popup;
    }
}