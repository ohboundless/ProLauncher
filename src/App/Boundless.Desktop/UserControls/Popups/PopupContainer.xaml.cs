using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Boundless.Desktop.UserControls.Popups;

public partial class PopupContainer : UserControl
{
    public PopupContainer()
    {
        InitializeComponent();
    }

    public void AddPopup(UserControl control, bool removeAllOthers = false)
    {
        if (removeAllOthers)
            Container.Children.Clear();
        
        Container.Children.Add(control);
        DoVisibilityCheck();
    }

    public void RemovePopup(UserControl control)
    {
        Container.Children.Remove(control);
        DoVisibilityCheck();
    }

    private void DoVisibilityCheck()
    {
        if (Container.Children.Count == 0)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        Visibility = Visibility.Visible;
    }

    private void Container_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Container.Children.RemoveAt(Container.Children.Count - 1);
        DoVisibilityCheck();
    }
}