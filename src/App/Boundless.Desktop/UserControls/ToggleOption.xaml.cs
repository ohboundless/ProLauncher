using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Boundless.Desktop.Config.Types;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls;

public partial class ToggleOption : UserControl
{
    private PropertyInfo _info;
    
    public ToggleOption(PropertyInfo propertyInfo, bool value)
    {
        _info = propertyInfo;
        
        InitializeComponent();
        SetInfo();

        ToggleBtn.IsChecked = value;
    }

    private void SetInfo()
    {
        var desc = _info.GetCustomAttribute<SettingsDescription>();
        if (desc == null)
        {
            OptionName.Text = _info.Name;
            OptionDesc.Text = string.Empty;
            return;
        }
        
        OptionName.Text = desc.Name;
        OptionDesc.Text = desc.Description;
    }

    private void ToggleButton_CheckChanged(object sender, RoutedEventArgs e)
    {
        _info.SetValue(Globals.Settings, ((ToggleButton)sender).IsChecked);
        Globals.Settings.Save();
    }
}