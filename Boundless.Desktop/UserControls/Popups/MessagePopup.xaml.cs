using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Boundless.Desktop.UserControls.Popups;

public partial class MessagePopup : UserControl
{
    public PopupContainer ParentGrid;

    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            TitleTextBlock.Text = value;
        }
    }
    
    private string _message;
    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            MessageTextBlock.Text = value;
        }
    }

    private string _confirmButtonText;
    public string ConfirmButtonText
    {
        get => _confirmButtonText;
        set
        {
            _confirmButtonText = value;
            ConfirmButton.Content = value;
        }
    }

    public Action? ConfirmAction;
    
    public MessagePopup(PopupContainer parentGrid, string title, string message, 
        string confirmButtonText = "Ok", Action? confirmAction = null)
    {
        InitializeComponent();
        
        ParentGrid = parentGrid;
        Title = title;
        Message = message;
        ConfirmButtonText = confirmButtonText;
        ConfirmAction = confirmAction;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
        ConfirmAction?.Invoke();
        RemovePopup();
    }
    
    public void RemovePopup() => ParentGrid.RemovePopup(this);
}