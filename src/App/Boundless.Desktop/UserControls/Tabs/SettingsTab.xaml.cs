using System.Windows.Controls;

namespace Boundless.Desktop.UserControls.Tabs;

public partial class SettingsTab : UserControl
{
    public SettingsTab()
    {
        InitializeComponent();
        PopulateSettings();
    }

    private void PopulateSettings()
    {
        var type = Settings.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(Settings);

            if (value is bool boolean)
            {
                var option = new ToggleOption(property, boolean);
                SettingsPanel.Children.Add(option);
            }
            else
            {
                throw new NotImplementedException("Cannot load property.");
            }
        }
    }
}