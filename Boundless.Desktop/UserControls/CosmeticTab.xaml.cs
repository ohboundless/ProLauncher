using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Boundless.Desktop.ViewModels;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticTab : UserControl
{
    public static readonly DependencyProperty CosmeticsViewModelProperty =
        DependencyProperty.Register(
            nameof(CosmeticsViewModel),
            typeof(CosmeticsViewModel),
            typeof(CosmeticTab),
            new PropertyMetadata(null));

    public CosmeticsViewModel? CosmeticsViewModel
    {
        get => (CosmeticsViewModel?)GetValue(CosmeticsViewModelProperty);
        set => SetValue(CosmeticsViewModelProperty, value);
    }

    public CosmeticTab()
    {
        InitializeComponent();
        
        CosmeticsViewModel = new CosmeticsViewModel();
        DataContext = CosmeticsViewModel;
    }

    // From ChatGPT cause I cba
    #region SCROLL BAR FIX
    private void ListBox_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not ListBox listBox)
            return;

        var scrollViewer = FindVisualChild<ScrollViewer>(listBox);

        if (scrollViewer == null)
            return;

        const double scrollAmount = 55;

        if (e.Delta > 0)
            scrollViewer.ScrollToVerticalOffset(
                Math.Max(0, scrollViewer.VerticalOffset - scrollAmount));
        else
            scrollViewer.ScrollToVerticalOffset(
                scrollViewer.VerticalOffset + scrollAmount);

        e.Handled = true;
    }

    private static T? FindVisualChild<T>(DependencyObject obj)
        where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);

            if (child is T result)
                return result;

            var descendant = FindVisualChild<T>(child);

            if (descendant != null)
                return descendant;
        }

        return null;
    }
    #endregion
}