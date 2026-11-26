using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls.Popups;

public partial class InputPopup : UserControl
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
    
    public InputPopup(string title, string confirmButtonText = "Confirm")
    {
        InitializeComponent();
        
        Title = title;
        ConfirmButtonText = confirmButtonText;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
        MainWindow.PopupContainer.RemovePopup(this);
        _confirmed = true;
    }
    
    public async Task<string> GetValueAsync()
    {
        await Dispatcher.InvokeAsync(() =>
        {
            MainWindow.PopupContainer.AddPopup(this);
        });
        
        while (!_confirmed)
        {
            await Task.Delay(150);
        }

        return TextBox.Text;
    }
}