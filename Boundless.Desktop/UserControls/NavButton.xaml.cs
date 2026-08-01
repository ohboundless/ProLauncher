using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Boundless.Desktop.UserControls;

public partial class NavButton : UserControl
{
    public string Text
    {
        get => _textBlock.Text;
        set => _textBlock.Text = value;
    }
    
    public SymbolRegular Icon
    {
        get => _labelIcon.Symbol;
        set => _labelIcon.Symbol = value;
    }

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_isActive)
            {
                _labelIcon.Filled = true;
                _textBlock.FontWeight = FontWeight.FromOpenTypeWeight(500);
                _btn.Background = new SolidColorBrush(Color.FromRgb(58, 67, 119));
            }
            else
            {
                _labelIcon.Filled = false;
                _textBlock.FontWeight = FontWeight.FromOpenTypeWeight(400);
                _btn.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
    }

    public NavButton()
    {
        InitializeComponent();
    }
}