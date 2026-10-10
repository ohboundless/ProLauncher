using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Boundless.Desktop.UserControls.Popups;

public partial class MultipleOptionsPopup : UserControl
{
    private readonly IEnumerable<KeyValuePair<string, Action>> _choicesAndActions;
    
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
    
    public MultipleOptionsPopup(string? title, string? message, params KeyValuePair<string, Action>[] choicesAndActions)
    {
        _choicesAndActions = choicesAndActions;
        
        InitializeComponent();
        PopulateOptions();

        if (string.IsNullOrEmpty(title))
            TextPanel.Children.Remove(TitleTextBlock);
        else
            Title = title;
        
        if (string.IsNullOrEmpty(message))
            TextPanel.Children.Remove(MessageTextBlock);
        else
            Message = message;
        
        
        MainWindow.PopupContainer.AddPopup(this);
    }

    private void PopulateOptions()
    {
        foreach (var choice in _choicesAndActions)
        {
            var button = new Button
            {
                Content = choice.Key,
                Padding = new Thickness(30, 10, 30, 10),
                Background = new SolidColorBrush(Color.FromArgb(4, 255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(58, 67, 119)),
                BorderThickness = new Thickness(1),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(3)
            };

            button.Click += (_, _) =>
            {
                choice.Value();
                Close();
            };

            OptionsPanel.Children.Add(button);
        }
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void Close() => MainWindow.PopupContainer.RemovePopup(this);
}