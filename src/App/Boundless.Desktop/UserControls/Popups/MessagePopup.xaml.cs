using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class MessagePopup : UserControl
{
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
    }

    public static void Show(string title, string message, string confirmButtonText = "Ok", Action? confirmAction = null)
    {
        MainWindow?.PopupContainer.ShowMessage(title, message, confirmButtonText, confirmAction);
    }
    
    public void RemovePopup() => ParentGrid.RemovePopup(this);
}