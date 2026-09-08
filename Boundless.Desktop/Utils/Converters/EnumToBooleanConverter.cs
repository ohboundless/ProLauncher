using System.Globalization;
using System.Windows.Data;

namespace Boundless.Desktop.Utils.Converters;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        return value.Equals(parameter);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (parameter == null)
            return Binding.DoNothing;

        return value is true ? parameter : Binding.DoNothing;
    }
}