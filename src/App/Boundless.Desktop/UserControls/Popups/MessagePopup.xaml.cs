using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class MessagePopup : UserControl
{
    public string Title
    {
        get;
        set
        {
            field = value;
            TitleTextBlock.Text = value;
        }
    }

    public string Message
    {
        get;
        set
        {
            field = value;
            MessageTextBlock.Text = value;
        }
    }

    public string ConfirmButtonText
    {
        get;
        set
        {
            field = value;
            ConfirmButton.Content = value;
        }
    }

    private bool _confirmed;
    public readonly Action? ConfirmAction;
    public PopupContainer? ParentGrid { get; set; }
    
    public MessagePopup(string title, string message, 
        string confirmButtonText = "Ok", Action? confirmAction = null, PopupContainer? parentGrid = null)
    {
        InitializeComponent();
        
        Title = title;
        Message = message;
        ConfirmButtonText = confirmButtonText;
        ConfirmAction = confirmAction;
        ParentGrid = parentGrid;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
        ParentGrid ??= MainWindow!.PopupContainer;
        ConfirmAction?.Invoke();
        RemovePopup();

        _confirmed = true;
    }

    public static MessagePopup? Show(string title, string message, string confirmButtonText = "Ok", Action? confirmAction = null)
    {
        return MainWindow?.PopupContainer.ShowMessage(title, message, confirmButtonText, confirmAction);
    }

    public async Task WaitAsync()
    {
        while (!_confirmed)
        {
            await Task.Delay(150);
        }
    }
    
    public void RemovePopup() => ParentGrid.RemovePopup(this);
}