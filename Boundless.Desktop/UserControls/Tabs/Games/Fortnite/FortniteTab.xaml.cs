using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Boundless.Desktop.Windows.Fortnite;

namespace Boundless.Desktop.UserControls.Tabs.Games.Fortnite;

public partial class FortniteTab : TabWithBack
{
    public FortniteTab(Action backAction) : base(backAction)
    {
        InitializeComponent();
    }

    private void BackArrow_MouseUp(object sender, MouseButtonEventArgs e)
    {
        BackAction();
    }

    private void AccountCheckerBtn_OnClick(object sender, RoutedEventArgs e)
    {
        new FortniteLockerWindow().Show();
    }
}