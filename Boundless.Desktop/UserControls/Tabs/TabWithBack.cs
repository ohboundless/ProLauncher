using System.Windows.Controls;
using System.Windows.Input;

namespace Boundless.Desktop.UserControls.Tabs;

public partial class TabWithBack : UserControl
{
    public Action BackAction { get; set; }
    
    public TabWithBack(Action backAction)
    {
        BackAction = backAction;
    }
}