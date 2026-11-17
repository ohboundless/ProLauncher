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
            var propertyType = property.PropertyType;
            var value = property.GetValue(Settings);

            if (propertyType == typeof(bool))
            {
                var boolean = (bool)value!;
                var option = new ToggleOption(property, boolean);
                SettingsPanel.Children.Add(option);
            }
            else if (propertyType == typeof(string))
            {
                var str = (string?)value ?? string.Empty;
                var option = new StringOption(property, str);
                SettingsPanel.Children.Add(option);
            }
            else
            {
                throw new NotImplementedException("Cannot load property.");
            }
        }
    }
}