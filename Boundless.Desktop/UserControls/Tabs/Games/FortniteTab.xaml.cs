using System.Windows.Controls;
using System.Windows.Input;

namespace Boundless.Desktop.UserControls.Tabs.Games;

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
}