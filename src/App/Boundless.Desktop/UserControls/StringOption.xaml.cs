using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Boundless.Desktop.Config.Types;
using Boundless.Desktop.Utils;
using Microsoft.Win32;

namespace Boundless.Desktop.UserControls;

public partial class StringOption : UserControl
{
    private PropertyInfo _info;
    private SettingsFlags _flags;
    
    public StringOption(PropertyInfo propertyInfo, string value)
    {
        _info = propertyInfo;
        
        InitializeComponent();
        SetInfo();

        TextBox.Text = value;
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

        _flags = desc.Flags;
        if (!_flags.HasFlag(SettingsFlags.DirectorySelector)
            && !_flags.HasFlag(SettingsFlags.FileSelector))
        {
            SelectDirBtn.Visibility = Visibility.Collapsed;
            Grid.SetColumnSpan(TextBox, 2);
        }
    }
    
    private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _info.SetValue(Globals.Settings, ((TextBox)sender).Text);
        Globals.Settings.Save();
    }

    private void SelectDirBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (_flags.HasFlag(SettingsFlags.DirectorySelector))
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                TextBox.Text = dialog.FolderName;
            }
        }
        else if (_flags.HasFlag(SettingsFlags.FileSelector))
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                TextBox.Text = dialog.FileName;
            }
        }
    }
}