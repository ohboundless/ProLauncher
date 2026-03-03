using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Boundless.Desktop.UserControls;

public partial class NavButton : UserControl
{
    public string Label
    {
        get => (string)ToolTip;
        set => ToolTip = value;
    }
    
    public SymbolRegular Icon
    {
        get => LabelIcon.Symbol;
        set => LabelIcon.Symbol = value;
    }

    public NavButton()
    {
        InitializeComponent();
    }
}