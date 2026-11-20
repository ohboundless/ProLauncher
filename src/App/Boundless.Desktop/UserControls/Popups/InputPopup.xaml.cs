using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class InputPopup : UserControl
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

    private Action<string>? ConfirmAction;
    
    public InputPopup(PopupContainer parentGrid, string title, 
        string confirmButtonText = "Confirm", Action<string>? confirmAction = null)
    {
        InitializeComponent();
        
        ParentGrid = parentGrid;
        Title = title;
        ConfirmButtonText = confirmButtonText;
        ConfirmAction = confirmAction;

        parentGrid.Container.Children.Add(this);
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
        ConfirmAction?.Invoke(TextBox.Text);
        RemovePopup();
    }
    
    public void RemovePopup() => ParentGrid.RemovePopup(this);
}