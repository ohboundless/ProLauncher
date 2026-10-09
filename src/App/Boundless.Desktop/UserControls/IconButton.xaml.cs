using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Boundless.Desktop.UserControls;

public partial class IconButton : UserControl
{
    public IconButton()
    {
        InitializeComponent();
        border.MouseUp += Border_OnMouseUp;
    }

    public event RoutedEventHandler? Click;
    
    private void Border_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        Click?.Invoke(this, e);
    }
    
    public SymbolRegular Symbol
    {
        get => (SymbolRegular)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(
            nameof(Symbol),
            typeof(SymbolRegular),
            typeof(IconButton),
            new PropertyMetadata(SymbolRegular.Accessibility24));
    
    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(IconButton),
            new PropertyMetadata(null));
}