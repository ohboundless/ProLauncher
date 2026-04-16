using System.Windows.Controls;
using System.Windows.Input;
using Boundless.Desktop.UserControls.Tabs.Games;
using Boundless.Desktop.UserControls.Tabs.Games.Fortnite;

namespace Boundless.Desktop.UserControls.Tabs;

public partial class LibraryTab : UserControl
{
    public LibraryTab()
    {
        InitializeComponent();
    }

    private void Fortnite_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        MainWindow.SetCurrentTab<FortniteTab>(() => MainWindow.SetCurrentTab<LibraryTab>());
    }
}